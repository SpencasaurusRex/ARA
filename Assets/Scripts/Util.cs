using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARACore
{
    public class Util
    {
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

        public static Quaternion ToQuaternion(Heading h)
        {
            return Quaternion.AngleAxis(-90 * (int)h, Vector3.up);
        }

        public static int EuclideanMod(int a, int b)
        {
            Console.WriteLine("{0} % {1} = {2}", a, b, ((a % b) + b) % b);
            return ((a % b) + b) % b;
        }
    }
}
