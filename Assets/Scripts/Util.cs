using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARACore
{
    public class Util
    {
        public static Quaternion ToQuaternion(int h)
        {
            return Quaternion.AngleAxis(-90 * h, Vector3.up);
        }

        public static int EuclideanMod(int a, int b)
        {
            return ((a % b) + b) % b;
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

        // A: FUBD
        // H: ENWS
        // D: ENWSUD
    }

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
}
