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
        public static int HEADING_EAST = 0;
        public static int HEADING_NORTH = 1;
        public static int HEADING_WEST = 2;
        public static int HEADING_SOUTH = 3;

        public enum TileEntityActionResult : byte
        {
            Success,
            Blocked,
            OutOfFuel,
            AlreadyMoving,
        }

        public enum TileEntityAction : int
        {
            Forward,
            Up,
            Back,
            Down,
            TurnLeft,
            TurnRight,
            Idle
        }

        #region Fields
        public const uint MAX_ENTITIES = 1020;
        public const int CHUNK_LENGTH_X = 200;
        public const int CHUNK_LENGTH_Z = 110;
        public const int CHUNK_HEIGHT = 9;

        static uint currentTileEntityId;
        // Who is currently registered to move into a spot
        static Dictionary<Vector3Int, uint> registeredMoves = new Dictionary<Vector3Int, uint>(new Vector3IntEqualityComparer());
        // Tiles that are blocked
        public static Dictionary<Vector3Int, uint> blocked = new Dictionary<Vector3Int, uint>(new Vector3IntEqualityComparer());

        // TileEntity fields
        public static TileObject[] tileObject = new TileObject[MAX_ENTITIES];
        public static int[] movementTime = new int[MAX_ENTITIES];
        public static int[] turningTime = new int[MAX_ENTITIES];
        public static int[] currentMovingTicks = new int[MAX_ENTITIES];
        public static int[] currentTurningTicks = new int[MAX_ENTITIES];
        public static Vector3Int[] currentPosition = new Vector3Int[MAX_ENTITIES];
        public static int[] currentHeading = new int[MAX_ENTITIES];
        public static Vector3Int[] targetPosition = new Vector3Int[MAX_ENTITIES];
        public static int[] targetHeading = new int[MAX_ENTITIES];
        public static TileEntityAction[] currentAction = new TileEntityAction[MAX_ENTITIES];
        public static TileEntityAction[] targetAction = new TileEntityAction[MAX_ENTITIES];
        public static TileEntityActionResult[] actionResult = new TileEntityActionResult[MAX_ENTITIES];

        // Non-system fields
        public static Vector3Int[] pixelTarget = new Vector3Int[MAX_ENTITIES];
        #endregion

        #region Private Methods
        private static bool IsBlocked(Vector3Int loc)
        {
            return blocked.ContainsKey(loc);
        }

        private static void Unblock(Vector3Int loc)
        {
            if (!blocked.ContainsKey(loc))
            {
                throw new Exception("Attempt to unblock " + loc.ToString() + " that wasn't blocked");
            }
            blocked.Remove(loc);
        }

        private static bool InBounds(Vector3Int loc)
        {
            return loc.x >= 0 && loc.x < CHUNK_LENGTH_X && loc.y >= 0 && loc.y < CHUNK_HEIGHT && loc.z >= 0 && loc.z < CHUNK_LENGTH_Z;
        }
        #endregion

        private static TileEntityAction Head(int currentHeading, int targetHeading)
        {
            // TODO
            //int dh = targetHeading - currentHeading;
            return TileEntityAction.TurnRight;
        }

        #region Interface Methods
        public static void ControlEntities()
        {

            // Control Logic
            for (uint i = 0; i < currentTileEntityId; i++)
            {
                if ( currentAction[i] == TileEntityAction.Idle)
                {
                    // If the last action was unsuccessful, do a random move
                    if (actionResult[i] == TileEntityActionResult.Blocked && UnityEngine.Random.value < .1f)
                    {
                        int a = UnityEngine.Random.Range(0, 4);
                        RegisterAction(i, (TileEntityAction)a);
                        continue;
                    }


                    Vector3Int dm = pixelTarget[i] - currentPosition[i];
                    int totalDistance = Mathf.Abs(dm.x) + Mathf.Abs(dm.y) + Mathf.Abs(dm.z);
                    if (totalDistance == 0) { 
                        continue;
                    }
                    float chanceX = (float)Mathf.Abs(dm.x) / totalDistance;
                    float chanceY = chanceX + (float)Mathf.Abs(dm.y) / totalDistance;
                    float r = UnityEngine.Random.value;

                    if (r < chanceX)
                    {
                        if (dm.x < 0)
                        {
                            if (currentHeading[i] != HEADING_WEST)
                            {
                                RegisterAction(i, Head(currentHeading[i], HEADING_WEST));
                                continue;
                            }
                            else
                            {
                                RegisterAction(i, TileEntityAction.Forward);
                                continue;
                            }
                        }
                        else if (dm.x > 0)
                        {
                            if (currentHeading[i] != HEADING_EAST)
                            {
                                RegisterAction(i, Head(currentHeading[i], HEADING_EAST));
                                continue;
                            }
                            else
                            {
                                RegisterAction(i, TileEntityAction.Forward);
                                continue;
                            }
                        }
                    }
                    else if (r < chanceY)
                    {
                        if (dm.y < 0)
                        {
                            RegisterAction(i, TileEntityAction.Down);
                            continue;
                        }
                        else if (dm.y > 0)
                        {
                            RegisterAction(i, TileEntityAction.Up);
                            continue;
                        }
                    }
                    else
                    {
                        if (dm.z < 0)
                        {
                            if (currentHeading[i] != HEADING_SOUTH)
                            {
                                RegisterAction(i, Head(currentHeading[i], HEADING_SOUTH));
                                continue;
                            }
                            else
                            {
                                RegisterAction(i, TileEntityAction.Forward);
                                continue;
                            }
                        }
                        else if (dm.z > 0)
                        {
                            if (currentHeading[i] != HEADING_NORTH)
                            {
                                RegisterAction(i, Head(currentHeading[i], HEADING_NORTH));
                                continue;
                            }
                            else
                            {
                                RegisterAction(i, TileEntityAction.Forward);
                                continue;
                            }
                        }
                    }
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
            // TODO prevent this from being called multiple times a frame
#if DEBUG
            if (action < 0 || (int)action >= 6) throw new Exception("Incorrect action");
#endif
            if (currentAction[id] != TileEntityAction.Idle || action == TileEntityAction.Idle)
            {
                actionResult[id] = TileEntityActionResult.AlreadyMoving;
                return;
            }
            if (action == TileEntityAction.TurnLeft)
            {
                targetHeading[id] = (targetHeading[id] + 1) % 4;
                currentAction[id] = action;
                actionResult[id] = TileEntityActionResult.Success;
                return;
            }
            else if (action == TileEntityAction.TurnRight)
            {
                targetHeading[id] = Util.EuclideanMod(targetHeading[id] - 1, 4);
                currentAction[id] = action;
                actionResult[id] = TileEntityActionResult.Success;
                return;
            }
            else
            {
                // Heading = 0: East, 1: North, 2: West, 3:South
                // Action =  0: Forward, 1: Up, 2: Back, 3: Down, TurnLeft, TurnRight
                int h = currentHeading[id];
                int a = (int)action;
                int horizontalActionModifier = (Mathf.Abs(a - 2) - 1);
                targetPosition[id].x = currentPosition[id].x + horizontalActionModifier * (Mathf.Abs((int)h - 2) - 1);
                targetPosition[id].y = currentPosition[id].y - Mathf.Abs(a - 1) + 1;
                targetPosition[id].z = currentPosition[id].z + horizontalActionModifier * (-Mathf.Abs((int)h - 1) + 1);

                var targetLoc = targetPosition[id];

                if (!InBounds(targetLoc) || IsBlocked(targetLoc))
                {
                    //Console.WriteLine("{0} can't move. {1},{2},{3} is blocked", id, targetX, targetY, targetZ);
                    actionResult[id] = TileEntityActionResult.Blocked;
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
                    if (theirTargetAction == TileEntityAction.Down) goto Blocked;
                    if (ourTargetAction == TileEntityAction.Up) goto Overwrite;
                    if (targetAction[otherId] == TileEntityAction.Up) goto Blocked;
                    if (currentHeading[id] > currentHeading[otherId]) goto Blocked;
                }
                else if (ourMovementTime > theirMovementTime)
                {
                    goto Blocked;
                }
            }
            Overwrite:
            //Console.WriteLine("Movement registered for " + id);
            registeredMoves[targetLoc] = id;
            actionResult[id] = TileEntityActionResult.Success;
            return;
            Blocked:
            actionResult[id] = TileEntityActionResult.Blocked;
       }
        #endregion
    }
}