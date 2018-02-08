using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class Vector3IntEqualityComparer : IEqualityComparer<Vector3Int>
    {
        public bool Equals(Vector3Int a, Vector3Int b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public int GetHashCode(Vector3Int obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class MovementManager
    {
        public enum TileEntityAction
        {
            Idle = -1,
            Forward = 0,
            Up,
            Back,
            Down,
            TurnLeft,
            TurnRight,
        }

        #region Fields
        const uint MAX_ENTITIES = 2048 * 4;
        public const int CHUNK_LENGTH = 64;
        public const int CHUNK_HEIGHT = 32;

        static uint currentTileEntityId;
        // Who is currently registered to move into a spot

        static Dictionary<Vector3Int, uint> registeredMoves = new Dictionary<Vector3Int, uint>(new Vector3IntEqualityComparer());
        // Tiles that are blocked
        public static Dictionary<Vector3Int, uint> blocked = new Dictionary<Vector3Int, uint>(new Vector3IntEqualityComparer());

        // TileEntity fields
        static TileObject[] tileObject = new TileObject[MAX_ENTITIES];
        static int[] movementTime = new int[MAX_ENTITIES];
        static int[] turningTime = new int[MAX_ENTITIES];
        static int[] currentMovingTicks = new int[MAX_ENTITIES];
        static int[] currentTurningTicks = new int[MAX_ENTITIES];
        static Vector3Int[] currentPosition = new Vector3Int[MAX_ENTITIES];
        static int[] currentHeading = new int[MAX_ENTITIES];
        static Vector3Int[] targetPosition = new Vector3Int[MAX_ENTITIES];
        static int[] targetHeading = new int[MAX_ENTITIES];
        static TileEntityAction[] currentAction = new TileEntityAction[MAX_ENTITIES];
        static TileEntityAction[] targetAction = new TileEntityAction[MAX_ENTITIES];
        #endregion

        #region Private Methods
        private static bool IsBlocked(Vector3Int loc)
        {
            return blocked.ContainsKey(loc);
        }

        private static void Unblock(Vector3Int loc)
        {
            blocked.Remove(loc);
        }

        private static bool InBounds(Vector3Int loc)
        {
            return loc.x >= 0 && loc.x < CHUNK_LENGTH && loc.y >= 0 && loc.y < CHUNK_LENGTH && loc.z >= 0 && loc.z < CHUNK_LENGTH;
        }
        #endregion

        #region Interface Methods
        public static void ControlEntities()
        {
            // Control Logic
            for (uint i = 0; i < currentTileEntityId; i++)
            {
                if (i == 0)
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        RegisterAction(i, TileEntityAction.TurnRight);
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        RegisterAction(i, TileEntityAction.TurnLeft);
                    }
                    if (Input.GetKey(KeyCode.W))
                    {
                        RegisterAction(i, TileEntityAction.Forward);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        RegisterAction(i, TileEntityAction.Back);
                    }
                    else if (Input.GetKey(KeyCode.Q))
                    {
                        RegisterAction(i, TileEntityAction.Up);
                    }
                    else if (Input.GetKey(KeyCode.E))
                    {
                        RegisterAction(i, TileEntityAction.Down);
                    }
                }
                else if (currentAction[i] == TileEntityAction.Idle)
                {
                    //TileEntityAction action = TileEntityAction.Forward;
                    TileEntityAction action = (TileEntityAction)UnityEngine.Random.Range(-1, 6);
                    RegisterAction(i, action);
                }
            }
        }

        public static void Tick()
        {
            var keys = registeredMoves.Keys;
            foreach (var destination in keys)
            {
                uint id = registeredMoves[destination];
                currentAction[id] = targetAction[id];

                blocked.Add(destination, id);
            }
            
            // Simulate movement
            for (uint i = 0; i < currentTileEntityId; i++)
            {
                TileEntityAction action = currentAction[i];
                if (action == TileEntityAction.Idle)
                {
                    continue;
                }
                // If the action is a movement
                if (action <= TileEntityAction.Down && action >= TileEntityAction.Forward)
                {
                    int currentTicks = ++currentMovingTicks[i];
                    int totalTicks = movementTime[i];
                    Vector3Int currentLoc = currentPosition[i];
                    tileObject[i].transform.position = Vector3.Lerp(currentLoc, targetPosition[i], (float)currentTicks / totalTicks);
                    if (currentTicks >= totalTicks)
                    {
                        Unblock(currentLoc);

                        currentMovingTicks[i] = 0;
                        currentPosition[i] = targetPosition[i];
                        currentAction[i] = TileEntityAction.Idle;
                    }
                }
                // If the action is a turn
                if (action == TileEntityAction.TurnLeft || action == TileEntityAction.TurnRight)
                {
                    int currentTicks = ++currentTurningTicks[i];
                    int totalTicks = turningTime[i];
                    Quaternion currentRot = Util.ToQuaternion(currentHeading[i]);
                    Quaternion targetRot = Util.ToQuaternion(targetHeading[i]);
                    tileObject[i].transform.rotation = Quaternion.Lerp(currentRot, targetRot, (float)currentTicks / totalTicks);
                    if (currentTicks >= totalTicks)
                    {
                        currentTurningTicks[i] = 0;
                        currentAction[i] = TileEntityAction.Idle;
                        currentHeading[i] = targetHeading[i];
                    }
                }
            }
            registeredMoves.Clear();
        }

        public static bool RegisterTileEntity(TileObject o, Vector3Int startingLocation, int moveTime, int turnTime, int heading = 0)
        {
            if (IsBlocked(startingLocation) || currentTileEntityId >= MAX_ENTITIES)
            {
                GameObject.Destroy(o.gameObject);
                return false;
            }

            o.transform.position = startingLocation;
            o.transform.rotation = Util.ToQuaternion(heading);

            uint id = currentTileEntityId++;
            o.id = id;

            tileObject[id] = o;
            currentPosition[id] = startingLocation;

            movementTime[id] = moveTime;
            turningTime[id] = turnTime;
            currentHeading[id] = heading;
            currentAction[id] = TileEntityAction.Idle;

            blocked.Add(startingLocation, id);
            return true;
        }

        public static void RegisterAction(uint id, TileEntityAction action)
        {
#if DEBUG
            if ((int)action < -1 || (int)action >= 6) throw new Exception("Incorrect action");
#endif
            if (currentAction[id] != TileEntityAction.Idle || action == TileEntityAction.Idle)
            {
                return;
            }
            if (action == TileEntityAction.TurnLeft)
            {
                targetHeading[id] = (targetHeading[id] + 1) % 4;
                currentAction[id] = action;
            }
            else if (action == TileEntityAction.TurnRight)
            {
                targetHeading[id] = Util.EuclideanMod(targetHeading[id] - 1, 4);
                currentAction[id] = action;
            }
            else
            {
                // Heading = 0: East, 1: North, 2: West, 3:South
                // Action =  0: Forward, 1: Up, 2: Back, 3: Down, TurnLeft, TurnRight
                int h = currentHeading[id];
                int a = (int)action;
                int horizontalActionModifier = (Mathf.Abs(a - 2) - 1);
                targetPosition[id].x = currentPosition[id].x + horizontalActionModifier * (Mathf.Abs(h - 2) - 1);
                targetPosition[id].y = currentPosition[id].y - Mathf.Abs(a - 1) + 1;
                targetPosition[id].z = currentPosition[id].z + horizontalActionModifier * (-Mathf.Abs(h - 1) + 1);

                var targetLoc = targetPosition[id];
                if (!InBounds(targetLoc) || IsBlocked(targetLoc))
                {
                    //Console.WriteLine("{0} can't move. {1},{2},{3} is blocked", id, targetX, targetY, targetZ);
                    return;
                }
                // We're good to register the movement
                targetAction[id] = action;
                RegisterMove(id, targetLoc);
            }
        }

        public static void RegisterMove(uint id, Vector3Int targetLoc)
        {
            //Console.WriteLine("Attempt Register move for {0} at {1},{2},{3}", id, targetX, targetY, targetZ);
            // Calculate collision
            if (registeredMoves.ContainsKey(targetLoc))
            {
                uint otherId = registeredMoves[targetLoc];
                int ourMovementTime = movementTime[id];
                int theirMovementTime = movementTime[otherId];

                // First compare speeds, faster moving entities get to move first
                if (ourMovementTime == theirMovementTime)
                {
                    TileEntityAction ourTargetAction = targetAction[id];
                    TileEntityAction theirTargetAction = targetAction[otherId];
                    // Now check our direction as a last resort
                    // Down before up before, East, North, West
                    if (ourTargetAction == TileEntityAction.Down) goto Overwrite;
                    if (theirTargetAction == TileEntityAction.Down) return;
                    if (ourTargetAction == TileEntityAction.Up) goto Overwrite;
                    if (targetAction[otherId] == TileEntityAction.Up) return;
                    if (currentHeading[id] > currentHeading[otherId]) return;
                }
                else if (ourMovementTime > theirMovementTime)
                {
                    return;
                }
            }
            Overwrite:
            //Console.WriteLine("Movement registered for " + id);
            registeredMoves[targetLoc] = id;
        }
        #endregion
    }
}