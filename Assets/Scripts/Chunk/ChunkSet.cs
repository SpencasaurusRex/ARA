using System.Collections.Generic;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public class ChunkSet
    {
        Dictionary<ChunkCoords, Chunk> chunks = new Dictionary<ChunkCoords, Chunk>();

        public Block GetBlock(Vector3Int coord)
        {
            return GetChunk(coord).GetBlock(coord);
        }

        public void SetBlock(Vector3Int coord, Block b)
        {
            GetChunk(coord).SetBlock(coord, b);
        }

        public void SetTileEntity(Vector3Int coord, Block b, Entity tileEntity)
        {
            GetChunk(coord).SetBlock(coord, b, tileEntity);
        }

        Chunk GetChunk(Vector3Int globalBlockCoords)
        {
            ChunkCoords cc = ChunkCoords.FromBlockCoords(globalBlockCoords);
            Chunk c;
            if (chunks.ContainsKey(cc))
            {
                c = chunks[cc];
            }
            else
            {
                c = chunks[cc] = new Chunk();
                // TODO load chunk from file or generate
            }
            return c;
        }
    }
}
