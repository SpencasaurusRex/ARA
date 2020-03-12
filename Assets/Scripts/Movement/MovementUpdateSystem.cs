﻿using System.Collections.Generic;
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

        readonly EntitySet movementRequestSet;

        static readonly Vector3Int[] Directions = new[] {new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, -1), new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), };

        public MovementUpdateSystem(World world)
        {
            this.world = world;

            movementRequestSet = world.GetEntities().With<MovementRequest>().AsSet();
        }

        public void Update(float fractional)
        {
            // Only process if we're at the end of the frame
            if (fractional - 1f > float.Epsilon) return;

            var movementRequestEntities = movementRequestSet.GetEntities();
            foreach (var entity in movementRequestEntities)
            {
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
            }
        }

        public void EndTick()
        {
            CheckMovements();
            WriteResults();
        }

        void CheckMovements()
        {
            foreach (var movement in requestedMovements)
            {
                // TODO: Check for world collisions
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
                var result = new MovementResult
                {
                    Result = !movement.Blocked
                };
                movement.Entity.Set(result);
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
