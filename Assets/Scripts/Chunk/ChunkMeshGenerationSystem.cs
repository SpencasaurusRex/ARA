using System.Collections.Generic;
using Assets.Scripts.Core;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public class ChunkMeshGenerationSystem
    {
        Material material;
        World world;
        EntitySet generateSet;
        EntitySet globalSet;
        ChunkSet chunkSet;
        BlockProperties properties;

        Vector3 chunkLocation;

        public ChunkMeshGenerationSystem(World world, Material material)
        {
            this.world = world;
            this.material = material;

            generateSet = world.GetEntities().With<Chunk>().With<GenerateMesh>().AsSet();
            globalSet = world.GetEntities().With<Global>().AsSet();
        }

        int triangleOffset = 0;

        public void Update()
        {
            var global = globalSet.GetEntities()[0];
            chunkSet = global.Get<ChunkSet>();
            properties = global.Get<BlockProperties>();

            foreach (var entity in generateSet.GetEntities())
            {
                triangleOffset = 0;
                var coord = entity.Get<GenerateMesh>().Coords;

                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();
                List<Vector2> uvs = new List<Vector2>();
                List<Vector3> normals = new List<Vector3>();

                int baseX = coord.X * Chunk.ChunkSize;
                int baseY = coord.Y * Chunk.ChunkSize;
                int baseZ = coord.Z * Chunk.ChunkSize;

                for (int z = baseZ; z < baseZ + Chunk.ChunkSize; z++)
                {
                    for (int y = baseY; y < baseY + Chunk.ChunkSize; y++)
                    {
                        for (int x = baseX; x < baseX + Chunk.ChunkSize; x++)
                        {
                            Block type = chunkSet.GetBlock(new Vector3Int(x, y, z));
                            if (!properties.Values[type].GenerateMesh) continue;

                            Vector3 position = new Vector3(x, y, z);
                            // TODO: Check for blocks across chunks, but only if the chunk is rendered!
                            if (properties.Values[chunkSet.GetBlock(new Vector3Int(x + 1, y + 0, z + 0))].Transparent) CreateQuad(position, type, BlockSide.East, vertices, triangles, uvs, normals);
                            if (properties.Values[chunkSet.GetBlock(new Vector3Int(x + 0, y + 0, z + 1))].Transparent) CreateQuad(position, type, BlockSide.North, vertices, triangles, uvs, normals);
                            if (properties.Values[chunkSet.GetBlock(new Vector3Int(x - 1, y + 0, z + 0))].Transparent) CreateQuad(position, type, BlockSide.West, vertices, triangles, uvs, normals);
                            if (properties.Values[chunkSet.GetBlock(new Vector3Int(x + 0, y + 0, z - 1))].Transparent) CreateQuad(position, type, BlockSide.South, vertices, triangles, uvs, normals);
                            if (properties.Values[chunkSet.GetBlock(new Vector3Int(x + 0, y + 1, z + 0))].Transparent) CreateQuad(position, type, BlockSide.Top, vertices, triangles, uvs, normals);
                            if (properties.Values[chunkSet.GetBlock(new Vector3Int(x + 0, y - 1, z + 0))].Transparent) CreateQuad(position, type, BlockSide.Bottom, vertices, triangles, uvs, normals);
                        }
                    }
                }

                Mesh mesh = new Mesh
                {
                    vertices = vertices.ToArray(), 
                    triangles = triangles.ToArray(),
                    uv = uvs.ToArray()
                };
                mesh.RecalculateBounds();

                entity.Set(mesh);
                entity.Set(material);
            }
        }

        static Vector3[] Offsets = new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1)
        };

        void CreateQuad(Vector3 pos, Block type, BlockSide side, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals)
        {
            int tileIndex = properties.Values[type].TileIndex[(int)side];
            uvs.AddRange(GetUVs(tileIndex));
            
            var to = triangleOffset;
            triangles.AddRange(new[] {to, to+1, to+2, to+1, to+3, to+2});

            switch (side)
            {
                case BlockSide.Bottom:
                    vertices.AddRange(new[] {Offsets[0]+pos, Offsets[1]+pos, Offsets[4]+pos, Offsets[5]+pos});
                    normals.AddRange(new[] {Vector3.down, Vector3.down, Vector3.down, Vector3.down});
                    break;
                case BlockSide.Top:
                    vertices.AddRange(new[] {Offsets[6]+pos, Offsets[7]+pos, Offsets[2]+pos, Offsets[3]+pos});
                    normals.AddRange(new[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up});
                    break;
                case BlockSide.West:
                    vertices.AddRange(new[] {Offsets[6]+pos, Offsets[2]+pos, Offsets[4]+pos, Offsets[0]+pos});
                    normals.AddRange(new[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left});
                    break;
                case BlockSide.East:
                    vertices.AddRange(new[] {Offsets[3]+pos, Offsets[7]+pos, Offsets[1]+pos, Offsets[5]+pos});
                    normals.AddRange(new[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right});
                    break;
                case BlockSide.North:
                    vertices.AddRange(new[] {Offsets[7]+pos, Offsets[6]+pos, Offsets[5]+pos, Offsets[4]+pos});
                    normals.AddRange(new[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward});
                    break;
                case BlockSide.South:
                    vertices.AddRange(new[] {Offsets[2]+pos, Offsets[3]+pos, Offsets[0]+pos, Offsets[1]+pos});
                    normals.AddRange(new[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back});
                    break;
            }

            triangleOffset += 4;
        }

        Vector2[] GetUVs(int uvTileIndex)
        {
            const int CANVAS_SIZE = 4;
            const float BLOCK_SIZE = 1f / CANVAS_SIZE;
            int x = uvTileIndex % CANVAS_SIZE;
            float u = x * BLOCK_SIZE;
            int y = uvTileIndex / CANVAS_SIZE;
            float v = y * BLOCK_SIZE;

            float e = 0f;

            Vector2 uv00 = new Vector2(u + e, v + e);
            Vector2 uv10 = new Vector2(u + BLOCK_SIZE - e, v + e);
            Vector2 uv01 = new Vector2(u + e, v + BLOCK_SIZE - e);
            Vector2 uv11 = new Vector2(u + BLOCK_SIZE - e, v + BLOCK_SIZE - e);
            return new Vector2[] {uv01, uv11, uv00, uv10};
        }
    }
}