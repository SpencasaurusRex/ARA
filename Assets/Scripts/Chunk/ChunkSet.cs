using System.Collections.Generic;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public class ChunkSet
    {
        public List<ChunkCoords> ChunksToLoad = new List<ChunkCoords>();
        
        Dictionary<ChunkCoords, Chunk> loadedChunks = new Dictionary<ChunkCoords, Chunk>();
        Dictionary<ChunkCoords, Entity> chunkEntities = new Dictionary<ChunkCoords, Entity>();
        World world;
        BlockProperties properties;

        public ChunkSet(World world, BlockProperties properties)
        {
            this.world = world;
            this.properties = properties;
        }

        public void UnloadChunkLogically(ChunkCoords coords)
        {
            // Assume this chunk is already unloaded visually
            var chunkEntity = chunkEntities[coords];
            chunkEntity.Dispose();

            loadedChunks.Remove(coords);
            chunkEntities.Remove(coords);
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
            
            if (!properties.Values[(int)b].GenerateMesh) return;
            var cc = new ChunkCoords(coord);
            var entity = GetChunkEntity(cc);
            if (!entity.Has<GenerateMesh>())
            {
                entity.Set(new GenerateMesh
                {
                    Coords = cc
                });
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

        public Chunk GetChunk(ChunkCoords coords)
        {
            Chunk chunk;
            if (loadedChunks.ContainsKey(coords))
            {
                chunk = loadedChunks[coords];
            }
            else
            {
                var entity = world.CreateEntity();
                chunk = loadedChunks[coords] = new Chunk(coords);
                chunkEntities[coords] = entity;
                entity.Set(chunk);

                if (false /* TODO: on disk? */)
                {
                    // Load from disk
                }
                else
                {
                    GenerateChunk(chunk, coords);
                }
            }
            return chunk;
        }

        public bool IsChunkLoaded(ChunkCoords coords) => loadedChunks.ContainsKey(coords);

        void GenerateChunk(Chunk chunk, ChunkCoords coords)
        {
            if (coords.Y == -1)
            {
                for (int x = 0; x < Chunk.ChunkSize; x++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (int z = 0; z < Chunk.ChunkSize; z++)
                        {
                            if (y == Chunk.ChunkSize - 2)
                            {
                                chunk.SetBlockLocal(x, y, z, Block.Grass);
                            }
                            else if (y < Chunk.ChunkSize - 2)
                            {
                                chunk.SetBlockLocal(x, y, z, Block.Dirt);
                            }
                        }
                    }
                }
            }
        }
    }
}
