using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARACore
{
    public class Chunk
    {
        public static Material BlockMaterial;

        public const int CHUNK_SIZE_X = 1 << ChunkCoords.CHUNK_SHIFT_X;
        public const int CHUNK_SIZE_Y = 1 << ChunkCoords.CHUNK_SHIFT_Y;
        public const int CHUNK_SIZE_Z = 1 << ChunkCoords.CHUNK_SHIFT_Z;

        public const int CHUNK_MASK_X = CHUNK_SIZE_X - 1;
        public const int CHUNK_MASK_Y = CHUNK_SIZE_Y - 1;
        public const int CHUNK_MASK_Z = CHUNK_SIZE_Z - 1;

        public const int CHUNK_SIZE = CHUNK_SIZE_X * CHUNK_SIZE_Y * CHUNK_SIZE_Z;

        private Block[] blocks;
        private Mesh mesh;
        private ChunkCoords coords;

        public Chunk(ChunkSet world, ChunkCoords cc)
        {
            blocks = new Block[CHUNK_SIZE];
            coords = cc;

            // Generate world
            if (cc.cy == -1)
            {
                for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
                {
                    for (int z = 0; z < Chunk.CHUNK_SIZE_Z; z++)
                    {
                        Block block;
                        block.id = world.CurrentBlockId;
                        block.type = BlockType.Grass;
                        blocks[GetIndexFromLocal(x, CHUNK_SIZE_Y - 1, z)] = block;
                    }
                }
            }
            GenerateMesh();
        }

        public Block GetBlock(int gx, int gy, int gz)
        {
            return blocks[GetIndexFromGlobal(gx, gy, gz)];
        }

        public void SetBlock(int gx, int gy, int gz, Block block)
        {
            blocks[GetIndexFromGlobal(gx, gy, gz)] = block;
            GenerateMesh();
        }

        public Mesh GenerateMesh()
        {
            return null;
        }

        private static int GetIndexFromLocal(int lx, int ly, int lz)
        {
            return (lz << (ChunkCoords.CHUNK_SHIFT_Y + ChunkCoords.CHUNK_SHIFT_X)) + (ly << ChunkCoords.CHUNK_SHIFT_X) + lx;
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