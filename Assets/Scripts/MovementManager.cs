using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class MovementManager : MonoBehaviour
    {
        HashSet<TileObject> objects = new HashSet<TileObject>();

        HashSet<TileObject> up    = new HashSet<TileObject>();
        HashSet<TileObject> down  = new HashSet<TileObject>();
        HashSet<TileObject> east  = new HashSet<TileObject>();
        HashSet<TileObject> north = new HashSet<TileObject>();
        HashSet<TileObject> west  = new HashSet<TileObject>();
        HashSet<TileObject> south = new HashSet<TileObject>();
        bool[,,] blocked = new bool[32, 32, 32];

        #region Unity Methods
        void OnDrawGizmos()
        {
            // Display blocked grid
            for (int i = 0; i < blocked.GetLength(0); i++)
            {
                for (int j = 0; j < blocked.GetLength(1); j++)
                {
                    for (int k = 0; k < blocked.GetLength(2); k++)
                    {
                        if (blocked[i, j, k])
                        {
                            Gizmos.color = new Color(1, 1, 0, 0.75F);
                            Gizmos.DrawCube(new Vector3(i, j, k), Vector3.one * .99f);
                        }
                        else
                        {
                            Gizmos.color = new Color(.2f, 1, .2f, 0.25F);
                            Gizmos.DrawCube(new Vector3(i, j, k), Vector3.one * .1f);
                        }
                    }
                }
            }
        }

        void FixedUpdate()
        {
            up.Clear();
            down.Clear();
            east.Clear();
            north.Clear();
            west.Clear();
            south.Clear();

            foreach (var obj in objects)
            {
                obj.Tick();
            }

            // Collapse all foreach into single with Dictionay<Vector3Int, HashSet<TileObject>> requestedMoves
            foreach (var obj in up)
            {
                var targetPosition = obj.position + Util.ToVector3Int(Direction.Up);
                Debug.Log("Target position: " + targetPosition);
                if (InBounds(targetPosition) && !blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    // TODO set velocity for interpolation/extrapolation? Remember to set it back too
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in down)
            {
                var targetPosition = obj.position + Util.ToVector3Int(Direction.Down);
                Debug.Log("Target position: " + targetPosition);
                if (InBounds(targetPosition) && !blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in east)
            {
                var targetPosition = obj.position + Util.ToVector3Int(Direction.East);
                Debug.Log("Target position: " + targetPosition);
                if (InBounds(targetPosition) && !blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in north)
            {
                var targetPosition = obj.position + Util.ToVector3Int(Direction.North);
                Debug.Log("Target position: " + targetPosition);
                if (InBounds(targetPosition) && !blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in west)
            {
                var targetPosition = obj.position + Util.ToVector3Int(Direction.West);
                Debug.Log("Target position: " + targetPosition);
                if (InBounds(targetPosition) && !blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in south)
            {
                var targetPosition = obj.position + Util.ToVector3Int(Direction.South);
                Debug.Log("Target position: " + targetPosition);
                if (InBounds(targetPosition) && !blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
        }
        #endregion

        private bool InBounds(Vector3Int pos)
        {
            return pos.x >= 0 && pos.x < blocked.GetLength(0) && pos.y >= 0 && pos.y < blocked.GetLength(1) && pos.z >= 0 && pos.z < blocked.GetLength(2);
        }

        #region API Methods
        /// <summary>
        /// Registers with the manager that the TileObject wants to take the targetAction that is currently set
        /// </summary>
        /// <param name="o">The TileObject to register the action for</param>
        public void RegisterAction(TileObject o)
        {
            switch (o.targetAction)
            {
                case MovementAction.Up:
                    up.Add(o);
                    break;
                case MovementAction.GoDown:
                    down.Add(o);
                    break;
                case MovementAction.TurnLeft:
                    o.action = MovementAction.TurnLeft;
                    o.targetHeading = (Heading)(((int)o.heading + 1) % 4);
                    break;
                case MovementAction.TurnRight:
                    o.action = MovementAction.TurnRight;
                    o.targetHeading = (Heading)Util.EuclideanMod((int)o.heading - 1, 4);
                    break;
                case MovementAction.GoForward:
                    if (o.heading == Heading.East)
                    {
                        east.Add(o);
                    }
                    if (o.heading == Heading.North)
                    {
                        north.Add(o);
                    }
                    if (o.heading == Heading.West)
                    {
                        west.Add(o);
                    }
                    if (o.heading == Heading.South)
                    {
                        south.Add(o);
                    }
                    break;
                case MovementAction.GoBack:
                    if (o.heading == Heading.East)
                    {
                        west.Add(o);
                    }
                    if (o.heading == Heading.North)
                    {
                        south.Add(o);
                    }
                    if (o.heading == Heading.West)
                    {
                        east.Add(o);
                    }
                    if (o.heading == Heading.South)
                    {
                        north.Add(o);
                    }
                    break;
            }
        }

        public void RegisterTileObject(TileObject obj)
        {
            if (blocked[obj.position.x, obj.position.y, obj.position.z])
            {
                Debug.LogError("Create TileObject at blocked location: " + obj.position.ToString() + " " + obj.ToString());
            }
            objects.Add(obj);
            blocked[obj.position.x, obj.position.y, obj.position.z] = true;
        }

        public void Unblock(Vector3Int pos)
        {
            if (!blocked[pos.x, pos.y, pos.z])
            {
                Debug.LogError("Attempting to unblock a tile that was already unblocked: " + pos.ToString());
            }
            blocked[pos.x, pos.y, pos.z] = false;
        }
        #endregion
    }
}