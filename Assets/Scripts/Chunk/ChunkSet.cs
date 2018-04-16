using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ARACore
{
    public class ChunkSet : MonoBehaviour
    {
        public ChunkMesh chunkMeshPrefab;

        ulong currentBlockId;
        public ulong CurrentBlockId
        {
            get
            {
                return currentBlockId++;
            }
        }

        Dictionary<ChunkCoords, Chunk> chunks = new Dictionary<ChunkCoords, Chunk>();

        void Awake()
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int z = -2; z <= 2; z++)
                {
                    GenerateChunk(new ChunkCoords(x, -1, z));    
                }
            }
        }

        private void Update()
        {
            // TODO: Move mesh logic into a MeshManager
            // TODO: Update active mesh renderers based on camera's position
        }

        public void GenerateChunk(ChunkCoords cc)
        {
            // Make sure the chunk doesn't already exist
            Chunk chunk;
            if (!chunks.TryGetValue(cc, out chunk))
            {
                chunk = new Chunk(this, cc);
                chunks[cc] = chunk;
            }
            chunk.GenerateMesh();
        }

        public void CreateBlock(int gx, int gy, int gz, BlockType type)
        {
            Block b;
            // TODO only use this if the block needs other data components (ex: Update)
            b.id = CurrentBlockId;
            b.type = type;
            GetChunk(gx, gy, gz).SetBlock(gx, gy, gz, b);
        }

        public Block GetBlock(Int64 gx, Int64 gy, Int64 gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz);
        }

        public ulong GetBlockId(Int64 gx, Int64 gy, Int64 gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz).id;
        }

        public BlockType GetBlockType(Int64 gx, Int64 gy, Int64 gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz).type;
        }

        public bool IsAir(Int64 gx, Int64 gy, Int64 gz)
        {
            return GetBlockType(gx, gy, gz) == BlockType.Air;
        }

        private Chunk GetChunk(Int64 gx, Int64 gy, Int64 gz)
        {
            ChunkCoords cc = ChunkCoords.FromBlockCoords(gx, gy, gz);
            Chunk c;
            if (chunks.ContainsKey(cc))
            {
                c = chunks[cc];
            }
            else
            {
                c = chunks[cc] = new Chunk(this, cc);
            }
            return c;
        }
    }
}
