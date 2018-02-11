using NUnit.Framework;
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

        public void SetBlockType(int gx, int gy, int gz, BlockType b)
        {
            GetChunk(gx, gy, gz).SetBlock(gx, gy, gz, b);
        }

        public BlockType GetBlockType(int gx, int gy, int gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz);
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
