using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARACore
{
    public class Util
    {
        public static Vector3Int ToVector3Int(MovementAction m, Heading h)
        {
            switch (m)
            {
                case MovementAction.GoUp: return Vector3Int.up;
                case MovementAction.GoDown: return Vector3Int.down;
                case MovementAction.GoForward: return ToVector3Int(h);
                case MovementAction.GoBack: return Negate(ToVector3Int(h));
            }
            throw new Exception("Unhandled MovementAction: " + m.ToString());
        }

        public static Vector3Int Negate(Vector3Int v)
        {
            return new Vector3Int(-v.x, -v.y, -v.z);
        }

        public static Vector3Int ToVector3Int(Heading h)
        {
            switch (h)
            {
                case Heading.East: return Vector3Int.right;
                case Heading.North: return new Vector3Int(0, 0, 1);
                case Heading.West: return Vector3Int.left;
                case Heading.South: return new Vector3Int(0, 0, -1);
            }
            throw new Exception("Unhandled Heading: " + h.ToString());
        }

        //public static Vector3Int ToVector3Int(Direction d)
        //{
        //    switch (d)
        //    {
        //        case Direction.Up: return Vector3Int.up;
        //        case Direction.Down: return Vector3Int.down;
        //        case Direction.East: return Vector3Int.right;
        //        case Direction.West: return Vector3Int.left;
        //        case Direction.North: return new Vector3Int(0, 0, 1);
        //        case Direction.South: return new Vector3Int(0, 0, -1);
        //    }
        //    throw new Exception("Unhandled direction: " + d.ToString());
        //}

        public static Quaternion ToQuaternion(int h)
        {
            return Quaternion.AngleAxis(-90 * h, Vector3.up);
        }

        public static int EuclideanMod(int a, int b)
        {
            return ((a % b) + b) % b;
        }
    }
}
