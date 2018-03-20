using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public enum MovementAction
    {
        Forward,
        Up,
        Back,
        Down,
        TurnLeft,
        TurnRight
    }

    public enum Direction
    {
        East,
        North,
        West,
        South,
        Up,
        Down,
    }

    public class MovementManager
    {
        #region Movement Related Types
        public struct MovementEntity
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3Int tilePosition;
            public int heading;
            public int ticksPerTile;
            public int ticksPerTurn;
        }

        private struct TileMove
        {
            public ulong id;
            public int progress;
            public Vector3Int startingTile;
            public Vector3Int targetTile;
            public Direction direction;
            // TODO: Benchark using startingLoc, targetLoc here
        }

        private struct Turn
        {
            public ulong id;
            public int progress;
            public int targetHeading;
            public Quaternion startingRotation;
            public Quaternion targetRotation;
        }

        private struct MovementCheck
        {
            public Direction direction;
            public Vector3 tilePosition;
        }
        #endregion

        #region EqualityComparers
        private class MovementCheckEqualityComparer : IEqualityComparer<MovementCheck>
        {
            public bool Equals(MovementCheck x, MovementCheck y)
            {
                return x.tilePosition == y.tilePosition && x.direction == y.direction;
            }

            public int GetHashCode(MovementCheck mc)
            {
                return mc.tilePosition.GetHashCode() * mc.direction.GetHashCode() << 4;
            }
        }
        #endregion

        Dictionary<ulong, MovementEntity> movementEntities;
        Dictionary<Vector3Int, TileMove> tileMoveRequests;
        Dictionary<ulong, TileMove> awardedMoves;
        Dictionary<ulong, Turn> awardedTurns;
        Dictionary<MovementCheck, ulong> forwardChecks;
        
        public MovementManager()
        {
            movementEntities = new Dictionary<ulong, MovementEntity>();
            tileMoveRequests = new Dictionary<Vector3Int, TileMove>(new Vector3IntEqualityComparer());
            awardedMoves = new Dictionary<ulong, TileMove>();
            awardedTurns = new Dictionary<ulong, Turn>();
            forwardChecks = new Dictionary<MovementCheck, ulong>(new MovementCheckEqualityComparer());
        }

        public void RegisterTileEntity(TileEntity entity)
        {
            var movementEntity = new MovementEntity();
            movementEntity.position = entity.transform.position;
            movementEntity.rotation = entity.transform.rotation;            
            movementEntity.tilePosition = Vector3Int.FloorToInt(entity.transform.position);
            movementEntities.Add(entity.id, movementEntity);
        }

        public void RequestMovement(ulong id, MovementAction action)
        {
            MovementEntity movementEntity = movementEntities[id];
            if (action == MovementAction.TurnLeft || action == MovementAction.TurnRight)
            {
                // TODO optimize this
                int targetHeading = movementEntity.heading;
                if (action == MovementAction.TurnLeft)
                {
                    targetHeading = (targetHeading + 1) % 4;
                }
                if (action == MovementAction.TurnRight)
                {
                    targetHeading = targetHeading - 1;
                    if (targetHeading == -1)
                    {
                        targetHeading = 3;
                    }
                }

                Turn turn = new Turn();
                turn.id = id;
                turn.startingRotation = Util.ToQuaternion(movementEntity.heading);
                turn.targetRotation = Util.ToQuaternion(targetHeading);
                turn.targetHeading = targetHeading;

                awardedTurns.Add(id, turn);
                return;
            }

            // TODO calculate target
            Vector3Int targetTile = movementEntity.tilePosition;
            Direction direction = Util.ToDirection(action, movementEntity.heading);

            // Check for other movers
            // TODO: Check block system
            //if (!ChunkSet.World.IsAir())
            //{
            //    // TODO: Check forwardChecks
            //    if (false)
            //    {
            //        return;
            //    }
            //}

            TileMove move = new TileMove();
            move.id = id;
            move.startingTile = movementEntity.tilePosition;
            move.targetTile = targetTile;

            if (tileMoveRequests.ContainsKey(targetTile))
            {
                // Determine priority
                // TODO
                if (true)
                {
                    tileMoveRequests[targetTile] = move;
                }
            }
            else
            {
                // Nothing was in our way
                tileMoveRequests[targetTile] = move;
            }
        }

        public void Tick()
        {
            // Phase 2 - Assignment
            foreach (var request in tileMoveRequests)
            {
                TileMove tileMove = request.Value;
                awardedMoves.Add(request.Value.id, request.Value);

                // TODO: Create ghost block
                // TODO: Create forward check
                //forwardChecks.Add(tileMove.id, );
                tileMoveRequests.Remove(request.Key);
            }

            // Phase 3 - Movement
            foreach (var movements in awardedMoves)
            {
                TileMove move = movements.Value;
                MovementEntity entity = movementEntities[move.id];
                if (move.progress >= entity.ticksPerTile)
                {
                    MovementCheck check = new MovementCheck();
                    check.direction = move.direction;
                    check.tilePosition = move.targetTile;
                    forwardChecks.Remove(check);

                    // TODO: clean up ghost block

                    awardedMoves.Remove(move.id);
                    return;
                }
                float movementProgress = (float)move.progress / entity.ticksPerTile;
                entity.position = Vector3.Lerp(move.startingTile, move.targetTile, movementProgress);
            }

            foreach (var turns in awardedTurns)
            {
                Turn turn = turns.Value;
                MovementEntity entity = movementEntities[turn.id];
                if (turn.progress >= entity.ticksPerTurn)
                {
                    entity.heading = turn.targetHeading;
                    awardedTurns.Remove(turn.id);
                    return;
                }
                float turningProgress = (float)turn.progress / entity.ticksPerTurn;
                entity.rotation = Quaternion.Slerp(turn.startingRotation, turn.targetRotation, turningProgress);
            }
        }
    }
}