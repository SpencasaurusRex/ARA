using System.Collections.Generic;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public class ChunkSet
    {
        Dictionary<ChunkCoords, Chunk> chunks = new Dictionary<ChunkCoords, Chunk>();
        Dictionary<ChunkCoords, Entity> chunkEntities = new Dictionary<ChunkCoords, Entity>();
        World world;
        BlockProperties properties;

        public ChunkSet(World world, BlockProperties properties)
        {
            this.world = world;
            this.properties = properties;
        }

        public Block GetBlock(Vector3Int coord)
        {
            return GetChunk(coord).GetBlock(coord);
        }

        public void SetBlock(Vector3Int coord, Block b)
        {
            var preBlock = GetChunk(coord).GetBlock(coord);

            if (preBlock == b) return;
            GetChunk(coord).SetBlock(coord, b);
            
            if (!properties.Values[b].GenerateMesh) return;
            var entity = GetChunkEntity(new ChunkCoords(coord));
            if (!entity.Has<GenerateMesh>())
            {
                entity.Set<GenerateMesh>();
            }
        }

        public Entity GetChunkEntity(ChunkCoords coord)
        {
            GetChunk(coord);
            return chunkEntities[coord];
        }

        public void SetTileEntity(Vector3Int coord, Block b, Entity tileEntity)
        {
            GetChunk(coord).SetBlock(coord, b, tileEntity);
        }

        Chunk GetChunk(Vector3Int globalBlockCoords) => GetChunk(new ChunkCoords(globalBlockCoords));

        Chunk GetChunk(ChunkCoords cc)
        {
            Chunk c;
            if (chunks.ContainsKey(cc))
            {
                c = chunks[cc];
            }
            else
            {
                var entity = world.CreateEntity();
                c = chunks[cc] = new Chunk(cc);
                chunkEntities[cc] = entity;
                entity.Set(c);
                // TODO load chunk from file or generate
            }
            return c;
        }
    }
}
