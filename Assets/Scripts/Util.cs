using System;
using System.Collections.Generic;
using Assets.Scripts.Core;
using DefaultEcs;
using UnityEngine;

namespace ARACore
{
    public enum Direction
    {
        East,
        North,
        West,
        South,
        Up,
        Down
    }

    public enum MovementAction
    {
        Forward,
        Up,
        Back,
        Down,
        TurnLeft,
        TurnRight
    }

    public static class Util
    {
        public static Vector3 ToVector3(this Vector3Int v) => new Vector3(v.x, v.y, v.z);
        public static Vector3Int Round(this Vector3 v) => Vector3Int.RoundToInt(v);
        public static Vector3 Translation(this Matrix4x4 mat) => new Vector3(mat.m03, mat.m13, mat.m23);
        public static Quaternion ToQuaternion(int h)
        {
            return Quaternion.AngleAxis(-90 * h, Vector3.up);
        }

        public static int EuclideanMod(int a, int b)
        {
            return ((a % b) + b) % b;
        }

        public static Vector3Int Forward = new Vector3Int(0, 0, 1);
        public static Vector3Int Back = new Vector3Int(0, 0, -1);

        public static Vector3Int ToVector3Int(Direction dir)
        {
            switch (dir)
            {
                case Direction.East: return Vector3Int.right;
                case Direction.North: return Forward;
                case Direction.West: return Vector3Int.left;
                case Direction.South: return Back;
                case Direction.Up: return Vector3Int.up;
                case Direction.Down: return Vector3Int.down;
                default: throw new ArgumentException("Invalid direction");
            }
        }

        public static Direction ToDirection(MovementAction action, int heading)
        {
            if (action == MovementAction.TurnLeft || action == MovementAction.TurnRight)
            {
                throw new ArgumentException("Do not call ToDirection with a turning action");
            }
            if (action == MovementAction.Up)
            {
                return Direction.Up;
            }
            if (action == MovementAction.Down)
            {
                return Direction.Down;
            }
            return (Direction)((int)(heading + action) % 4);
        }

        public static Entity GetGlobalEntity(this World world) => world.GetEntities().With<Global>().AsSet().GetEntities()[0];
    }
}
