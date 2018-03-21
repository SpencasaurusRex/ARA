using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ARACore
{
    public class ChunkSet
    {
        Dictionary<ChunkCoords, Chunk> chunks;

        public ChunkSet()
        {
            chunks = new Dictionary<ChunkCoords, Chunk>();
        }

        public void SetBlockType(Vector3Int gPosition, BlockType b)
        {
            GetChunk(gPosition.x, gPosition.y, gPosition.z).SetBlock(gPosition.x, gPosition.y, gPosition.z, b);
        }

        public void SetBlockType(int gx, int gy, int gz, BlockType b)
        {
            GetChunk(gx, gy, gz).SetBlock(gx, gy, gz, b);
        }

        public BlockType GetBlockType(int gx, int gy, int gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz);
        }

        public bool IsAir(Vector3Int gPosition)
        {
            return GetBlockType(gPosition.x, gPosition.y, gPosition.z) == BlockType.Air;
        }

        public bool IsAir(int gx, int gy, int gz)
        {
            return GetBlockType(gx, gy, gz) == BlockType.Air;
        }

        private Chunk GetChunk(int gx, int gy, int gz)
        {
            ChunkCoords cc = ChunkCoords.FromBlockCoords(gx, gy, gz);
            Chunk c;
            if (chunks.ContainsKey(cc))
            {
                c = chunks[cc];
            }
            else
            {
                c = chunks[cc] = new Chunk();
            }
            return c;
        }
    }
}
