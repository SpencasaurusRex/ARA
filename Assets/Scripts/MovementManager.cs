using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public enum MovementAction
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back,
        Idle,
    }

    public enum Direction
    {
        Up,
        Down,
        East,
        North,
        West,
        South
    }

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

            foreach (var obj in up)
            {
                var targetPosition = obj.position + ToVector3Int(Direction.Up);
                Debug.Log("Target position: " + targetPosition);
                if (!blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in down)
            {
                var targetPosition = obj.position + ToVector3Int(Direction.Down);
                Debug.Log("Target position: " + targetPosition);
                if (!blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in east)
            {
                var targetPosition = obj.position + ToVector3Int(Direction.East);
                Debug.Log("Target position: " + targetPosition);
                if (!blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in north)
            {
                var targetPosition = obj.position + ToVector3Int(Direction.North);
                Debug.Log("Target position: " + targetPosition);
                if (!blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in west)
            {
                var targetPosition = obj.position + ToVector3Int(Direction.West);
                Debug.Log("Target position: " + targetPosition);
                if (!blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
            foreach (var obj in south)
            {
                var targetPosition = obj.position + ToVector3Int(Direction.South);
                Debug.Log("Target position: " + targetPosition);
                if (!blocked[targetPosition.x, targetPosition.y, targetPosition.z])
                {
                    blocked[targetPosition.x, targetPosition.y, targetPosition.z] = true;
                    obj.targetPosition = targetPosition;
                    obj.action = obj.targetAction;
                }
            }
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
                case MovementAction.Down:
                    down.Add(o);
                    break;
                case MovementAction.Left:
                    // TODO add functionality for turning left
                    break;
                case MovementAction.Right:
                    // TODO add functionality for turning right
                    break;
                case MovementAction.Forward:
                    if (o.heading == TileObject.Heading.East)
                    {
                        east.Add(o);
                    }
                    if (o.heading == TileObject.Heading.North)
                    {
                        north.Add(o);
                    }
                    if (o.heading == TileObject.Heading.West)
                    {
                        west.Add(o);
                    }
                    if (o.heading == TileObject.Heading.South)
                    {
                        south.Add(o);
                    }
                    break;
                case MovementAction.Back:
                    if (o.heading == TileObject.Heading.East)
                    {
                        west.Add(o);
                    }
                    if (o.heading == TileObject.Heading.North)
                    {
                        south.Add(o);
                    }
                    if (o.heading == TileObject.Heading.West)
                    {
                        east.Add(o);
                    }
                    if (o.heading == TileObject.Heading.South)
                    {
                        north.Add(o);
                    }
                    break;
            }
        }

        public void RegisterTileObject(TileObject obj)
        {
            objects.Add(obj);
        }

        public void Unblock(Vector3Int pos)
        {
//#if DEBUG
            if (!blocked[pos.x, pos.y, pos.z])
            {
                throw new Exception("Attempting to unblock a tile that was already unblocked: " + pos.ToString());
            }
//#endif
            blocked[pos.x, pos.y, pos.z] = false;
        }
        #endregion

        #region Utility Methods
        public static Vector3Int ToVector3Int(Direction d)
        {
            switch (d)
            {
                case Direction.Up: return Vector3Int.up;
                case Direction.Down: return Vector3Int.down;
                case Direction.East: return Vector3Int.right;
                case Direction.West: return Vector3Int.left;
                case Direction.North: return new Vector3Int(0, 0, 1);
                case Direction.South: return new Vector3Int(0, 0, -1);
            }
            throw new Exception("Unhandled direction: " + d.ToString());
        }
        #endregion
    }
}