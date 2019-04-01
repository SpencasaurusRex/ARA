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

        ChunkSet world;
        Block[] blocks;
        ChunkCoords coords;
        ChunkMesh mesh;

        public Chunk(ChunkSet world, ChunkCoords cc)
        {
            this.world = world;
            blocks = new Block[CHUNK_SIZE];
            coords = cc;

            // Generate world
            if (cc.cy == -1 && cc.cx >= 0 && cc.cx < 12 && cc.cz >= 0 && cc.cz < 12)
            {
                for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
                {
                    for (int z = 0; z < Chunk.CHUNK_SIZE_Z; z++)
                    {
                        long gx = cc.cx * Chunk.CHUNK_SIZE_X + x;
                        long gz = cc.cz * Chunk.CHUNK_SIZE_Z + z;
                        int groundY = 15;
                        //int groundY = 10 + Mathf.RoundToInt(Mathf.PerlinNoise(gx * .04f + 1000, gz * .04f + 1000) * 5);
                        Block block;// TODO can't we just generate an ID based on position?
                        block.id = world.CurrentBlockId;
                        block.type = BlockType.Grass;
                        blocks[GetIndexFromLocal(x, groundY, z)] = block;
                        for (int y = groundY - 1; y >= 0; y--)
                        {
                            block.id = world.CurrentBlockId;
                            block.type = BlockType.Dirt;
                            blocks[GetIndexFromGlobal(x, y, z)] = block;
                        }
                    }
                }
            }
        }

        public void GenerateMesh()
        {
            if (mesh == null)
            {
                mesh = GameObject.Instantiate<ChunkMesh>(world.chunkMeshPrefab);
                mesh.name = "ChunkMesh" + coords.cx + "," + coords.cy + "," + coords.cz;
                mesh.transform.parent = world.transform;
                mesh.coords = coords;
                mesh.world = world;
            }
            mesh.GenerateMesh();
        }

        public Block GetBlock(Int64 gx, Int64 gy, Int64 gz)
        {
            return blocks[GetIndexFromGlobal(gx, gy, gz)];
        }

        public void SetBlock(Int64 gx, Int64 gy, Int64 gz, Block block)
        {
            blocks[GetIndexFromGlobal(gx, gy, gz)] = block;
            if (BlockProperties.Get(block.type).generateMesh)
            {
                GenerateMesh();
            }
        }

        static int GetIndexFromLocal(int lx, int ly, int lz)
        {
            return (lz << (ChunkCoords.CHUNK_SHIFT_Y + ChunkCoords.CHUNK_SHIFT_X)) + (ly << ChunkCoords.CHUNK_SHIFT_X) + lx;
        }

        static int GetIndexFromGlobal(Int64 gx, Int64 gy, Int64 gz)
        {
            int lx = (int)(gx & CHUNK_MASK_X);
            int ly = (int)(gy & CHUNK_MASK_Y);
            int lz = (int)(gz & CHUNK_MASK_Z);
            // Index in the form of z,y,x
            return (lz << (ChunkCoords.CHUNK_SHIFT_Y + ChunkCoords.CHUNK_SHIFT_X)) + (ly << ChunkCoords.CHUNK_SHIFT_X) + lx;
        }
    }
}