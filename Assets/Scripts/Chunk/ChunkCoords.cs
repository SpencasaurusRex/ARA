using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ARACore
{
    public class ChunkCoords : IEquatable<ChunkCoords>
    {
        public const int CHUNK_SHIFT_X = 5; // 2^5 = 32
        public const int CHUNK_SHIFT_Y = 5;
        public const int CHUNK_SHIFT_Z = 5; 

        public Int64 cx;
        public Int64 cy;
        public Int64 cz;

        public ChunkCoords(Int64 cx, Int64 cy, Int64 cz)
        {
            this.cx = cx;
            this.cy = cy;
            this.cz = cz;
        }

        private static Int64 Expand(Int64 w)
        {
            w &= 0x3ff;
            w = (w | (w << 16)) & 4278190335;
            w = (w | (w << 8)) & 251719695;
            w = (w | (w << 4)) & 3272356035;
            w = (w | (w << 2)) & 1227133513;
            return w;
        }

        public static ChunkCoords FromBlockCoords(Int64 gx, Int64 gy, Int64 gz)
        {
            return new ChunkCoords(gx >> CHUNK_SHIFT_X, gy >> CHUNK_SHIFT_Y, gz >> CHUNK_SHIFT_Z);
        }

        public override int GetHashCode()
        {
            // TODO find a way to use a 64bit HashCode
            return (Expand(cx) + (Expand(cy) << 1) + (Expand(cz) << 2)).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ChunkCoords))
            {
                return false;
            }
            return Equals(obj);
        }

        public bool Equals(ChunkCoords other)
        {
            return this.cx == other.cx && this.cy == other.cy && this.cz == other.cz;
        }
    }
}