using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ARACore
{
    public class ChunkSet : MonoBehaviour
    {
        ulong currentBlockId;
        public ulong CurrentBlockId
        {
            get
            {
                return currentBlockId++;
            }
        }

        Dictionary<ChunkCoords, Chunk> chunks = new Dictionary<ChunkCoords, Chunk>();

        private void Start()
        {

        }

        private void Update()
        {
            // TODO: Move mesh logic into a MeshManager
            // TODO: Update active mesh renderers based on camera's position
        }

        public void GenerateChunk(ChunkCoords cc)
        {
            // Make sure the chunk doesn't already exist
            if (chunks.ContainsKey(cc))
            {
                return;
            }

            Chunk chunk = new Chunk(this, cc);
            chunks[cc] = chunk;
        }

        public void CreateBlock(int gx, int gy, int gz, BlockType type)
        {
            Block b;
            // TODO only use this if the block needs other data components (ex: Update)
            b.id = CurrentBlockId;
            b.type = type;
            GetChunk(gx, gy, gz).SetBlock(gx, gy, gz, b);
        }

        public void CreateBlock(Vector3Int globalPosition, BlockType type)
        {
            CreateBlock(globalPosition.x, globalPosition.y, globalPosition.z, type);
        }

        public Block GetBlock(int gx, int gy, int gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz);
        }

        public ulong GetBlockId(int gx, int gy, int gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz).id;
        }

        public BlockType GetBlockType(int gx, int gy, int gz)
        {
            return GetChunk(gx, gy, gz).GetBlock(gx, gy, gz).type;
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
                c = chunks[cc] = new Chunk(this, cc);
            }
            return c;
        }
    }
}
