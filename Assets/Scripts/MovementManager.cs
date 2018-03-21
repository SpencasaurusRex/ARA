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
            public TileEntity tileEntity;
            public Vector3Int tilePosition;
            public int heading;
            public int ticksPerTile;
            public int ticksPerTurn;
        }

        private class TileMove
        {
            public ulong id;
            public int progress;
            public Vector3Int startingTile;
            public Vector3Int targetTile;
            public Direction direction;
            // TODO: Benchark using startingLoc, targetLoc here
        }

        private class Turn
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
            public Vector3Int tilePosition;
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
            movementEntity.heading = entity.startHeading;
            movementEntity.tilePosition = Vector3Int.FloorToInt(entity.transform.position);
            movementEntity.tileEntity = entity;
            movementEntity.ticksPerTile = entity.ticksPerTile;
            movementEntity.ticksPerTurn = entity.ticksPerTurn;
            movementEntities.Add(entity.id, movementEntity);
        }

        public void DestroyTileEntity(TileEntity entity)
        {
            movementEntities.Remove(entity.id);
        }

        public void RequestMovement(ulong id, MovementAction action)
        {
            // Check if we're already moving
            if (awardedMoves.ContainsKey(id) || awardedTurns.ContainsKey(id))
            {
                return;
            }

            // TODO Track return values
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

            Vector3Int targetTile = movementEntity.tilePosition;
            Direction direction = Util.ToDirection(action, movementEntity.heading);
            targetTile += Util.ToVector3Int(direction);

            TileMove move = new TileMove();
            move.id = id;
            move.startingTile = movementEntity.tilePosition;
            move.targetTile = targetTile;

            // Check for other movers
            if (!Manager.world.IsAir(targetTile))
            {
                MovementCheck forwardCheck;
                forwardCheck.tilePosition = targetTile;
                forwardCheck.direction = direction;
                ulong forwardId;
                if (forwardChecks.TryGetValue(forwardCheck, out forwardId))
                {
                    // Check speeds
                    TileMove forwardMove;
                    if (!awardedMoves.TryGetValue(forwardId, out forwardMove))
                    {
                        throw new Exception("A forward move data object existed without a corresponding awarded move");
                    }
                    MovementEntity forwardEntity;
                    if (!movementEntities.TryGetValue(forwardId, out forwardEntity))
                    {
                        throw new Exception("A forward move data object existed without a coreesponding movement entity");
                    }
                    int forwardTime = forwardEntity.ticksPerTile - forwardMove.progress;
                    if (forwardTime <= movementEntity.ticksPerTile)
                    {
                        // We're going to get there later, so it's all good
                        // Clean up the forward check
                        forwardChecks.Remove(forwardCheck);
                        tileMoveRequests[targetTile] = move;
                        //Debug.Log("forward check");
                        return;
                    }
                }
                else
                {
                    // There's something in front of us that isn't moving
                    //Debug.Log(targetTile + " blocked");
                    return;
                }
                return;
            }
            TileMove otherMove;
            if (tileMoveRequests.TryGetValue(targetTile, out otherMove))
            {
                var otherEntity = movementEntities[otherMove.id];
                // Determine priority
                if (movementEntity.ticksPerTile == otherEntity.ticksPerTile)
                {
                    if (direction < otherMove.direction)
                    {
                        tileMoveRequests[targetTile] = move;
                        return;
                    }
                    else
                    {
                        Debug.Log("lost priority");
                        return;
                    }
                }
                else if (movementEntity.ticksPerTile < otherEntity.ticksPerTile)
                {
                    // We're faster
                    tileMoveRequests[targetTile] = move;
                    return;
                }
                else
                {
                    // We're slower
                    Debug.Log("too slow");
                    return;
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
                awardedMoves.Add(request.Value.id, tileMove);

                Manager.world.SetBlockType(request.Value.targetTile, BlockType.Robot);
               
                MovementCheck forwardCheck;
                forwardCheck.direction = request.Value.direction;
                forwardCheck.tilePosition = request.Value.startingTile;
                try
                {
                    forwardChecks.Add(forwardCheck, tileMove.id);
                }
                catch
                {
                    Debug.Break();
                    Debug.Log("Lel wat at " + forwardCheck.tilePosition + " and " + forwardCheck.direction);
                }
            }
            tileMoveRequests.Clear();

            // Phase 3 - Movement
            List<ulong> doneMoves = new List<ulong>();
            foreach (var moveKVP in awardedMoves)
            {
                var id = moveKVP.Key;
                var move = moveKVP.Value;
                move.progress++;
                MovementEntity entity = movementEntities[id];
                if (move.progress >= entity.ticksPerTile)
                {
                    MovementCheck check = new MovementCheck();
                    check.direction = move.direction;
                    check.tilePosition = move.startingTile;
                    if (forwardChecks.ContainsKey(check))
                    {
                        forwardChecks.Remove(check);
                        Manager.world.SetBlockType(check.tilePosition, BlockType.Air);
                    }
                    movementEntities.Remove(id);
                    entity.tilePosition = move.targetTile;
                    entity.tileEntity.transform.position = move.targetTile;
                    movementEntities[id] = entity;
                    doneMoves.Add(move.id);
                    continue;
                }
                float movementProgress = (float)move.progress / entity.ticksPerTile;
                entity.tileEntity.transform.position = Vector3.Lerp(move.startingTile, move.targetTile, movementProgress);
            }
            foreach (var move in doneMoves)
            {
                awardedMoves.Remove(move);
            }

            List<ulong> doneTurns = new List<ulong>();
            foreach (var turns in awardedTurns)
            {
                Turn turn = turns.Value;
                turn.progress++;
                MovementEntity entity = movementEntities[turn.id];
                if (turn.progress >= entity.ticksPerTurn)
                {
                    movementEntities.Remove(turn.id);
                    entity.heading = turn.targetHeading;
                    entity.tileEntity.transform.rotation = turn.targetRotation;
                    movementEntities[turn.id] = entity;
                    doneTurns.Add(turn.id);
                    continue;
                }
                float turningProgress = (float)turn.progress / entity.ticksPerTurn;
                entity.tileEntity.transform.rotation = Quaternion.Slerp(turn.startingRotation, turn.targetRotation, turningProgress);
            }
            foreach (var turn in doneTurns)
            {
                awardedTurns.Remove(turn);
            }
        }
    }
}