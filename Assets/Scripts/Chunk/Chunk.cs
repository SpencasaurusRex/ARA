using System.Collections.Generic;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public class Chunk
    {
        public const int ChunkBits = 4;
        public const int ChunkSize = 1 << ChunkBits;
        public const int ChunkMask = ChunkSize - 1;
        const int NumBlocks = ChunkSize * ChunkSize * ChunkSize;

        public Dictionary<int, Entity> TileEntities;
        public Block[] Blocks;

        public Chunk(Block[] blocks, Dictionary<int, Entity> tileEntities)
        {
            Blocks = blocks;
            TileEntities = tileEntities;
        }

        public Chunk()
        {
            Blocks = new Block[NumBlocks];
            TileEntities = new Dictionary<int, Entity>();
        }

        public Block GetBlock(Vector3Int global)
        {
            return Blocks[GetIndexFromGlobal(global)];
        }

        public void SetBlock(Vector3Int global, Block block)
        {
            Blocks[GetIndexFromGlobal(global)] = block;
        }

        public void SetBlock(Vector3Int global, Block block, Entity entity)
        {
            int index = GetIndexFromGlobal(global);
            Blocks[index] = block;
            TileEntities[index] = entity;
        }

        static int GetIndexFromLocal(Vector3Int local)
        {
            return (local.z << (ChunkBits * 2)) + 
                   (local.y << (ChunkBits)) + 
                   (local.x);
        }

        static int GetIndexFromGlobal(Vector3Int global)
        {
            Vector3Int localCoords = new Vector3Int(
                global.x & ChunkMask,
                global.y & ChunkMask,
                global.z & ChunkMask
            );
            // Index in the form of z,y,x
            return GetIndexFromLocal(localCoords);
        }
    }
}