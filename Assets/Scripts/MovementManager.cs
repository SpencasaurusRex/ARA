﻿using System;
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

    public enum MovementResultType
    {
        DoneMoving,
        StartedMoving,
        Blocked,
        OutOfFuel,
        AlreadyMoving
    }

    public class MovementManager
    {
        public void DrawMoveResults()
        {
            foreach (var kvp in movementResult)
            {
                switch (kvp.Value.type)
                {
                    case MovementResultType.AlreadyMoving:
                        Gizmos.color = Color.yellow;
                        break;
                    case MovementResultType.Blocked:
                        Gizmos.color = Color.red;
                        break;
                    case MovementResultType.DoneMoving:
                        Gizmos.color = Color.green;
                        break;
                    case MovementResultType.OutOfFuel:
                        Gizmos.color = Color.gray;
                        break;
                    case MovementResultType.StartedMoving:
                        Gizmos.color = Color.cyan;
                        break;
                }
                Color c = Gizmos.color;
                c.a = 0.5f;
                Gizmos.color = c;
                RobotComponents rc = Manager.robotManager.Get(kvp.Key);
                Gizmos.DrawCube(rc.tileEntity.transform.position, Vector3.one);
            }
        }

        #region Movement Related Types
        public struct MovementResult
        {
            public MovementResult(MovementResultType type)
            {
                this.type = type;
            }

            public MovementResultType type;
        }

        public struct MovementEntity
        {
            public TileEntity tileEntity;
            public Vector3Int tilePosition;
            public int heading;
            public int ticksPerTile;
            public int ticksPerTurn;
        }

        class TileMove
        {
            public ulong id;
            public int progress;
            public Vector3Int startingTile;
            public Vector3Int targetTile;
            public Direction direction;
            // TODO: Benchark using startingLoc, targetLoc here
        }

        class Turn
        {
            public ulong id;
            public int progress;
            public int targetHeading;
            public Quaternion startingRotation;
            public Quaternion targetRotation;
        }

        public struct MovementCheck
        {
            public Direction direction;
            public Vector3Int tilePosition;
        }
        #endregion

        #region EqualityComparers
        class MovementCheckEqualityComparer : IEqualityComparer<MovementCheck>
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

        public Dictionary<ulong, MovementResult> movementResult;
        public Dictionary<ulong, MovementEntity> movementEntities;
        Dictionary<Vector3Int, TileMove> tileMoveRequests;
        Dictionary<ulong, TileMove> awardedMoves;
        Dictionary<ulong, Turn> awardedTurns; 
        public Dictionary<MovementCheck, ulong> forwardChecks;
        public Dictionary<MovementCheck, ulong> blockedMoves;
        ulong currentRequestId;

        public MovementManager()
        {
            movementResult = new Dictionary<ulong, MovementResult>();
            movementEntities = new Dictionary<ulong, MovementEntity>();
            tileMoveRequests = new Dictionary<Vector3Int, TileMove>(new Vector3IntEqualityComparer());
            awardedMoves = new Dictionary<ulong, TileMove>();
            awardedTurns = new Dictionary<ulong, Turn>();
            forwardChecks = new Dictionary<MovementCheck, ulong>(new MovementCheckEqualityComparer());
            blockedMoves = new Dictionary<MovementCheck, ulong>(new MovementCheckEqualityComparer());
        }

        public void RegisterTileEntity(TileEntity entity)
        {
            var movementEntity = new MovementEntity();
            movementEntity.heading = entity.StartHeading;
            movementEntity.tilePosition = Vector3Int.FloorToInt(entity.transform.position);
            movementEntity.tileEntity = entity;
            movementEntity.ticksPerTile = entity.TicksPerTile;
            movementEntity.ticksPerTurn = entity.TicksPerTurn;
            movementEntities.Add(entity.Id, movementEntity);
        }

        public void DestroyTileEntity(TileEntity entity)
        {
            movementEntities.Remove(entity.Id);
            movementResult.Remove(entity.Id);
        }

        public bool IsMoving(ulong id)
        {
            return awardedMoves.ContainsKey(id) || awardedTurns.ContainsKey(id);
        }

        public void RequestMovement(ulong id, MovementAction action)
        {
            //ulong requestId = currentRequestId++;

            // Check if we're already moving
            if (awardedMoves.ContainsKey(id) || awardedTurns.ContainsKey(id))
            {
                movementResult[id] = new MovementResult(MovementResultType.AlreadyMoving);
                return;
                //movementResult[requestId] = new MovementResult(MovementResultType.AlreadyMoving);
                //return requestId;
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
                movementResult[id] = new MovementResult(MovementResultType.StartedMoving);
                return;
                //movementResult[requestId] = new MovementResult(MovementResultType.StartedMoving);
                //return requestId;
            }

            Vector3Int targetTile = movementEntity.tilePosition;
            Direction direction = Util.ToDirection(action, movementEntity.heading);
            targetTile += Util.ToVector3Int(direction);

            TileMove move = new TileMove();
            move.id = id;
            move.startingTile = movementEntity.tilePosition;
            move.direction = direction;
            move.targetTile = targetTile;

            // Check for other movers
            if (!Manager.world.IsAir(targetTile.x, targetTile.y, targetTile.z))
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
                        throw new Exception("A forward check data object existed without a corresponding awarded move");
                    }
                    MovementEntity forwardEntity;
                    if (!movementEntities.TryGetValue(forwardId, out forwardEntity))
                    {
                        throw new Exception("A forward check data object existed without a corresponding movement entity");
                    }
                    int forwardTime = forwardEntity.ticksPerTile - forwardMove.progress;
                    if (forwardTime <= movementEntity.ticksPerTile)
                    {
                        // We're going to get there later, so it's all good
                        // Clean up the forward check
                        forwardChecks.Remove(forwardCheck);
                        tileMoveRequests[targetTile] = move;
                        return;
                    }
                }
                else
                {
                    // There's something in front of us that isn't moving
                    movementResult[id] = new MovementResult(MovementResultType.Blocked);
                    blockedMoves.Add(forwardCheck, id);
                    return;
                }

                // We were too fast
                movementResult[id] = new MovementResult(MovementResultType.Blocked);
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
                        // Lost priority
                        movementResult[id] = new MovementResult(MovementResultType.Blocked);
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
                    movementResult[id] = new MovementResult(MovementResultType.Blocked);
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
                ulong id = request.Value.id;
                awardedMoves.Add(id, tileMove);
                movementResult[id] = new MovementResult(MovementResultType.StartedMoving);

                Manager.world.CreateBlock(request.Value.targetTile.x, request.Value.targetTile.y, request.Value.targetTile.z, BlockType.Robot);

                // Iterative back-checking
                MovementCheck movementCheck;
                movementCheck.direction = request.Value.direction;
                movementCheck.tilePosition = request.Value.startingTile;
                ulong currentId = request.Value.id;
                Vector3Int dir = Util.ToVector3Int(movementCheck.direction);

                do
                {
                    ulong lastId = currentId;
                    if (blockedMoves.TryGetValue(movementCheck, out currentId) 
                        && movementEntities[currentId].ticksPerTile >= movementEntities[lastId].ticksPerTile)
                    {
                        // Award the move
                        TileMove move = new TileMove();
                        move.id = currentId;
                        move.targetTile = movementCheck.tilePosition;
                        // Move the position down the chain
                        move.startingTile = movementCheck.tilePosition -= dir;
                        move.direction = movementCheck.direction;
                        awardedMoves.Add(currentId, move);
                        movementResult[currentId] = new MovementResult(MovementResultType.StartedMoving);
                    }
                    else
                    {
                        forwardChecks.Add(movementCheck, lastId);
                        break;
                    }
                } while (true);
            }
            blockedMoves.Clear();
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
                        Manager.world.CreateBlock(check.tilePosition.x, check.tilePosition.y, check.tilePosition.z, BlockType.Air);
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
                movementResult[move] = new MovementResult(MovementResultType.DoneMoving);
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
                movementResult[turn] = new MovementResult(MovementResultType.DoneMoving);
                awardedTurns.Remove(turn);
            }
        }
    }
}