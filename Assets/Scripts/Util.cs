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
    }
}
