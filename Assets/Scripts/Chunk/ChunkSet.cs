using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Chunk
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

        public void GenerateWorld()
        {
            //for (int x = -13; x <= 13; x++)
            //{
            //    for (int z = -13; z <= 13; z++)
            //    {
            //        for (int y = -1; y <= 1; y++)
            //        {
            //            GenerateChunk(new ChunkCoords(x, y, z));
            //        }
            //    }
            //}
        }

        void Update()
        {
            // TODO: Move mesh logic into a MeshManager
            // TODO: Update active mesh renderers based on camera's position
        }

        public void GenerateChunk(ChunkCoords cc)
        {
            //// Make sure the chunk doesn't already exist
            //Chunk chunk;
            //if (!chunks.TryGetValue(cc, out chunk))
            //{
            //    chunk = new Chunk(this, cc);
            //    chunks[cc] = chunk;
            //}
            //chunk.GenerateMesh();
        }

        //public void CreateBlock(int gx, int gy, int gz, BlockType type)
        //{
        //    Block b;
        //    // TODO only use this if the block needs other data components (ex: Update)
        //    b.id = CurrentBlockId;
        //    b.type = type;
        //    GetChunk(gx, gy, gz).SetBlock(gx, gy, gz, b);
        //}

        //public Block GetBlock(Int64 gx, Int64 gy, Int64 gz)
        //{
        //    return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz);
        //}

        //public ulong GetBlockId(Int64 gx, Int64 gy, Int64 gz)
        //{
        //    return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz).id;
        //}

        //public BlockType GetBlockType(Int64 gx, Int64 gy, Int64 gz)
        //{
        //    return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz).type;
        //}

        //public bool IsAir(Int64 gx, Int64 gy, Int64 gz)
        //{
        //    return GetBlockType(gx, gy, gz) == BlockType.Air;
        //}

        //Chunk GetChunk(Int64 gx, Int64 gy, Int64 gz)
        //{
        //    ChunkCoords cc = ChunkCoords.FromBlockCoords(gx, gy, gz);
        //    Chunk c;
        //    if (chunks.ContainsKey(cc))
        //    {
        //        c = chunks[cc];
        //    }
        //    else
        //    {
        //        c = chunks[cc] = new Chunk(this, cc);
        //    }
        //    return c;
        //}
    }
}
