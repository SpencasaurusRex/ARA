using System;
using System.Collections.Generic;
using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class MovementUpdateSystem : IUpdateSystem
    {
        World world;

        readonly List<Movement> requestedMovements = new List<Movement>();
        readonly Dictionary<Vector3Int, Movement> movementFromLookup = new Dictionary<Vector3Int, Movement>();
        readonly Dictionary<Vector3Int, Vector3Int> movementDependency = new Dictionary<Vector3Int, Vector3Int>();

        readonly EntitySet globalSet;
        readonly EntitySet movementRequestSet;
        readonly EntitySet doneMovingSet;

        Entity[] entitiesToProcess;
        int lastIndexProcessed;

        static readonly Vector3Int[] Directions = {new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), };

        public MovementUpdateSystem(World world)
        {
            this.world = world;

            movementRequestSet = world.GetEntities().With<MovementRequest>().AsSet();
            doneMovingSet = world.GetEntities().With<Movement>().AsSet();
            globalSet = world.GetEntities().With<Global>().AsSet();
        }

        public void Update(float fractional)
        {
            if (Math.Abs(fractional - 1f) > float.Epsilon) return;

            entitiesToProcess = movementRequestSet.GetEntities().ToArray();
            
            var movementRequestEntities = movementRequestSet.GetEntities();
            foreach (var entity in movementRequestEntities)
            //for (int i = lastIndexProcessed; i < entitiesToProcess.Length * fractional; i++)
            {
                //Entity entity = entitiesToProcess[i];
                var request = entity.Get<MovementRequest>();
                var movement = new Movement
                {
                    Blocked = false,
                    Start = request.From,
                    Destination = request.To,
                    Direction = request.To - request.From,
                    Entity = entity
                };
                requestedMovements.Add(movement);
                movementFromLookup.Add(movement.Start, movement);
                //lastIndexProcessed = i;
            }
        }

        public void EndTick()
        {
            StopMoving();
            CheckMovements();
            WriteResults();
            requestedMovements.Clear();
            movementFromLookup.Clear();
            movementDependency.Clear();
            lastIndexProcessed = 0;
        }

        void CheckMovements()
        {
            var chunkSet = globalSet.GetEntities()[0].Get<ChunkSet>();

            foreach (var movement in requestedMovements)
            {
                if (movementFromLookup.TryGetValue(movement.Destination, out var obstacle))
                {
                    // If there is a mover in the way, and it's not moving in the same direction: block
                    if (obstacle.Direction != movement.Direction)
                    {
                        BlockMovement(movement);
                    } 
                    // If the mover in front of us is already blocked, so are we
                    else if (movementFromLookup.TryGetValue(movement.Destination, out var destMovement) && destMovement.Blocked)
                    {
                        BlockMovement(movement);
                    }
                    else
                    {
                        // Record dependency
                        movementDependency.Add(movement.Start, movement.Destination);
                    }
                }
                else if (chunkSet.GetBlock(movement.Destination) != Block.Air)
                {
                    BlockMovement(movement);
                }
                else
                {
                    foreach (var otherDirection in Directions)
                    {
                        if (-movement.Direction == otherDirection) continue;

                        var otherOrigin = movement.Destination + otherDirection;
                        if (movementFromLookup.TryGetValue(otherOrigin, out var otherMovement) && otherMovement.Destination == movement.Destination)
                        {
                            if (!otherMovement.Blocked)
                            {
                                BlockMovement(otherMovement);
                            }

                            if (!movement.Blocked)
                            {
                                BlockMovement(movement);
                            }
                        }
                    }
                }
            }
        }

        void WriteResults()
        {
            foreach (var movement in requestedMovements)
            {
                movement.Entity.Remove<MovementRequest>();

                var result = new ActionResult
                {
                    Result = !movement.Blocked
                };
                movement.Entity.Set(result);

                if (!movement.Blocked)
                {
                    movement.Entity.Set(new GridPosition { Value = movement.Destination });
                    movement.Entity.Set(new SetBlock{ Block = Block.Robot });
                    movement.Entity.Set(new RemoveBlock { Coords = movement.Start });
                }

                // TODO improve
                Movement movementComp;
                if (movement.Entity.Has<Movement>())
                {
                    if (movement.Blocked)
                    {
                        movement.Entity.Remove<Movement>();
                        continue;
                    }
                    movementComp = movement.Entity.Get<Movement>();
                }
                else if (!movement.Blocked)
                {
                    movement.Entity.Set(movement);
                    movementComp = movement;
                }
                else continue;

                movementComp.Start = movement.Start;
                movementComp.Destination = movement.Destination;
                movementComp.Direction = movement.Direction;
            }

        }

        void StopMoving()
        {
            foreach (var entity in doneMovingSet.GetEntities())
            {
                entity.Remove<Movement>();
            }
        }

        void BlockMovement(Movement movement)
        {
            while (true)
            {
                // Prevent duplicate calls
                if (movement.Blocked) return;
                movement.Blocked = true;
                // Check behind the mover
                var behind = movement.Start + movement.Start - movement.Destination;
                if (movementDependency.TryGetValue(behind, out var origin))
                {
                    // Propagate blocking down the chain
                    var direction = movement.Direction;
                    movement = movementFromLookup[behind];
                    if (movement.Direction != direction) return;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
