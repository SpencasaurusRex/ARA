using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public static class MovementManager
    {
        static List<TileObject> objects = new List<TileObject>();
        static Dictionary<Vector3Int, List<int>> registeredMoves = new Dictionary<Vector3Int, List<int>>();
        public static Dictionary<Vector3Int, int> blocked = new Dictionary<Vector3Int, int>();
        public const int CHUNK_LENGTH = 32;
        public const int CHUNK_HEIGHT = 10;

        static int currentId;

        #region Unity Methods
        public static void Tick()
        {
            registeredMoves.Clear();

            // Update all objects
            foreach (var obj in objects)
            {
                obj.Tick();
            }
            foreach (var loc in registeredMoves.Keys)
            {
                //Debug.Log("Target position: " + loc);
                int minPriority = int.MaxValue;

                int priorityId = -1;
                if (IsBlocked(loc))
                {
                    continue;
                }

                var movers = registeredMoves[loc];
                if (movers.Count == 1)
                {
                    var id = movers[0];
                    Block(loc, id);
                    var obj = objects[id];
                    obj.targetPosition = loc;
                    obj.action = obj.targetAction;
                    return;
                }

                foreach (var objectId in movers)
                {
                    Debug.Log("Almost Collision!");
                    var action = objects[objectId].targetAction;
                    // TODO: More complex priority logic
                    if ((int)action < minPriority)
                    {
                        minPriority = (int)action;
                        priorityId = objectId;
                    }
                }
                // If someone got priority
                if (priorityId >= 0)
                {
                    Block(loc, priorityId);
                    var obj = objects[priorityId];
                    obj.targetPosition = loc;
                    obj.action = obj.targetAction;
                }
            }
        }
        #endregion

        private static bool InBounds(Vector3Int pos)
        {
            return pos.x >= 0 && pos.x < CHUNK_LENGTH && pos.y >= 0 && pos.y < CHUNK_LENGTH && pos.z >= 0 && pos.z < CHUNK_LENGTH;
        }

        private static bool IsBlocked(Vector3Int location)
        {
            return blocked.ContainsKey(location);
        }

        private static void Block(Vector3Int location, int id)
        {
            // CHECK
            blocked.Add(location, id);
        }

        private static void QueueAction(Vector3Int location, TileObject obj)
        {
            if (!InBounds(location))
            {
                return;
            }
            if (!registeredMoves.ContainsKey(location))
            {
                registeredMoves.Add(location, new List<int>());
            }
            registeredMoves[location].Add(obj.id);
        }

        public static void Unblock(Vector3Int location)
        {
            blocked.Remove(location);
        }

        #region API Methods
        /// <summary>
        /// Registers with the manager that the TileObject wants to take the targetAction that is currently set
        /// </summary>
        /// <param name="o">The TileObject to register the action for</param>
        public static void RegisterAction(TileObject o)
        {
            switch (o.targetAction)
            {
                case MovementAction.GoUp:
                case MovementAction.GoDown:
                case MovementAction.GoForward:
                case MovementAction.GoBack:
                    QueueAction(o.position + Util.ToVector3Int(o.targetAction, o.heading), o);
                    break;
                case MovementAction.TurnLeft:
                    o.action = MovementAction.TurnLeft;
                    o.targetHeading = (Heading)(((int)o.heading + 1) % 4);
                    break;
                case MovementAction.TurnRight:
                    o.action = MovementAction.TurnRight;
                    o.targetHeading = (Heading)Util.EuclideanMod((int)o.heading - 1, 4);
                    break;
            }
        }

        public static void RegisterTileObject(TileObject obj)
        {
            if (IsBlocked(obj.position))
            {
                Debug.LogError("Create TileObject at blocked location: " + obj.position.ToString() + " " + obj.ToString());
            }
            obj.id = currentId++;
            objects.Add(obj);
            Block(obj.position, obj.id);
        }
        #endregion
    }
}