using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARACore
{
    public class Chunk
    {
        public const int CHUNK_SIZE_X = 1 << ChunkCoords.CHUNK_SHIFT_X;
        public const int CHUNK_SIZE_Y = 1 << ChunkCoords.CHUNK_SHIFT_Y;
        public const int CHUNK_SIZE_Z = 1 << ChunkCoords.CHUNK_SHIFT_Z;

        public const int CHUNK_MASK_X = CHUNK_SIZE_X - 1;
        public const int CHUNK_MASK_Y = CHUNK_SIZE_Y - 1;
        public const int CHUNK_MASK_Z = CHUNK_SIZE_Z - 1;

        public const int CHUNK_SIZE = CHUNK_SIZE_X * CHUNK_SIZE_Y * CHUNK_SIZE_Z;

        private BlockType[] blocks;

        public Chunk()
        {
            blocks = new BlockType[CHUNK_SIZE];
        }

        public BlockType GetBlock(int gx, int gy, int gz)
        {
            return blocks[GetIndexFromGlobal(gx, gy, gz)];
        }

        public void SetBlock(int gx, int gy, int gz, BlockType b)
        {
            blocks[GetIndexFromGlobal(gx, gy, gz)] = b;
        }

        private static int GetIndexFromGlobal(int gx, int gy, int gz)
        {
            int lx = gx & CHUNK_MASK_X;
            int ly = gy & CHUNK_MASK_Y;
            int lz = gz & CHUNK_MASK_Z;
            // Index in the form of z,y,x
            return (lz << (ChunkCoords.CHUNK_SHIFT_Y + ChunkCoords.CHUNK_SHIFT_X)) + (ly << ChunkCoords.CHUNK_SHIFT_X) + lx;
        }
    }
}
