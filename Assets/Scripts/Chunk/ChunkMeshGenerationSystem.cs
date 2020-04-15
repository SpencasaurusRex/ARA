using System.Collections.Generic;
using System.Diagnostics;
using ARACore;
using Assets.Scripts.Core;
using Assets.Scripts.Transform;
using Assets.Scripts.UnityComponents;
using DefaultEcs;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        Vector2[][] tileIndexUVs = new Vector2[CanvasSize * CanvasSize][];

        public ChunkMeshGenerationSystem(World world, Material material)
        {
            this.world = world;
            this.material = material;

            generateSet = world.GetEntities().With<Chunk>().With<GenerateMesh>().AsSet();
            globalSet = world.GetEntities().With<Global>().AsSet();

            PrecalculateUVs();
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
                var centerCoords = entity.Get<GenerateMesh>().Coords;

                List<Vector3> vertices = new List<Vector3>(1024);
                List<int> triangles = new List<int>(1024);
                List<Vector2> uvs = new List<Vector2>(1024);
                List<Vector3> normals = new List<Vector3>(1024);

                int baseX = centerCoords.X * Chunk.ChunkSize;
                int baseY = centerCoords.Y * Chunk.ChunkSize;
                int baseZ = centerCoords.Z * Chunk.ChunkSize;

                const int PadSize = Chunk.ChunkSize + 2;
                Block[,,] paddedBlocks = new Block[PadSize, PadSize, PadSize];
                bool[,,] paddedTransparent = new bool[PadSize, PadSize, PadSize];

                Stopwatch sw = Stopwatch.StartNew();

                // Fill x-
                var chunk = chunkSet.GetChunk(centerCoords.Offset(-1, 0, 0));
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        var block = paddedBlocks[0, y + 1, z + 1] = chunk.GetBlockLocal(Chunk.ChunkSize - 1, y, z);
                        paddedTransparent[0, y + 1, z + 1] = properties.Values[(int)block].Transparent;
                    }
                }

                // Fill x+
                chunk = chunkSet.GetChunk(centerCoords.Offset(1, 0, 0));
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        var block = paddedBlocks[Chunk.ChunkSize + 1, y + 1, z + 1] = chunk.GetBlockLocal(0, y, z);
                        paddedTransparent[Chunk.ChunkSize + 1, y + 1, z + 1] = properties.Values[(int)block].Transparent;
                    }
                }

                // Fill y-
                chunk = chunkSet.GetChunk(centerCoords.Offset(0, -1, 0));
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    for (int x = 0; x < Chunk.ChunkSize; x++)
                    {
                        var block = paddedBlocks[x + 1, 0, z + 1] = chunk.GetBlockLocal(x, Chunk.ChunkSize - 1, z);
                        paddedTransparent[x + 1, 0, z + 1] = properties.Values[(int)block].Transparent;
                    }
                }

                // Fill y+
                chunk = chunkSet.GetChunk(centerCoords.Offset(0, 1, 0));
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    for (int x = 0; x < Chunk.ChunkSize; x++)
                    {
                        var block = paddedBlocks[x + 1, Chunk.ChunkSize + 1, z + 1] = chunk.GetBlockLocal(x, 0, z);
                        paddedTransparent[x + 1, Chunk.ChunkSize + 1, z + 1] = properties.Values[(int)block].Transparent;
                    }
                }

                // Fill z-
                chunk = chunkSet.GetChunk(centerCoords.Offset(0, 0, -1));
                for (int y = 0; y < Chunk.ChunkSize; y++)
                {
                    for (int x = 0; x < Chunk.ChunkSize; x++)
                    {
                        var block = paddedBlocks[x + 1, y + 1, 0] = chunk.GetBlockLocal(x, y, Chunk.ChunkSize - 1);
                        paddedTransparent[x + 1, y + 1, 0] = properties.Values[(int)block].Transparent;
                    }
                }

                // Fill z+
                chunk = chunkSet.GetChunk(centerCoords.Offset(0, 0, 1));
                for (int y = 0; y < Chunk.ChunkSize; y++)
                {
                    for (int x = 0; x < Chunk.ChunkSize; x++)
                    {
                        var block = paddedBlocks[x + 1, y + 1, Chunk.ChunkSize + 1] = chunk.GetBlockLocal(x, y, 0);
                        paddedTransparent[x + 1, y + 1, Chunk.ChunkSize + 1] = properties.Values[(int)block].Transparent;
                    }
                }

                // Fill center
                chunk = chunkSet.GetChunk(centerCoords.Offset(0, 0, 0));
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (int x = 0; x < Chunk.ChunkSize; x++)
                        {
                            var block = paddedBlocks[x + 1, y + 1, z + 1] = chunk.GetBlockLocal(x, y, z);
                            paddedTransparent[x + 1, y + 1, z + 1] = properties.Values[(int) block].Transparent;
                        }
                    }
                }

                sw.Stop();
                long fill = sw.ElapsedMilliseconds;
                sw = Stopwatch.StartNew();
                for (int z = 0; z < Chunk.ChunkSize; z++)
                {
                    for (int y = 0; y < Chunk.ChunkSize; y++)
                    {
                        for (int x = 0; x < Chunk.ChunkSize; x++)
                        {
                            Block type = paddedBlocks[x + 1, y + 1, z + 1];
                            if (!properties.Values[(int)type].GenerateMesh) continue;
                            var props = properties.Values[(int)type];

                            Vector3 position = new Vector3(x, y, z);
                            if (paddedTransparent[x + 2, y + 1, z + 1])
                                CreateQuad(position, props, BlockSide.East, vertices, triangles, uvs, normals);
                            if (paddedTransparent[x + 1, y + 1, z + 2])
                                CreateQuad(position, props, BlockSide.North, vertices, triangles, uvs, normals);
                            if (paddedTransparent[x + 0, y + 1, z + 1])
                                CreateQuad(position, props, BlockSide.West, vertices, triangles, uvs, normals);
                            if (paddedTransparent[x + 1, y + 1, z + 0])
                                CreateQuad(position, props, BlockSide.South, vertices, triangles, uvs, normals);
                            if (paddedTransparent[x + 1, y + 2, z + 1])
                                CreateQuad(position, props, BlockSide.Top, vertices, triangles, uvs, normals);
                            if (paddedTransparent[x + 1, y + 0, z + 1])
                                CreateQuad(position, props, BlockSide.Bottom, vertices, triangles, uvs, normals);
                        }
                    }
                }

                sw.Stop();
                //Debug.Log(fill + " " + sw.ElapsedMilliseconds);

                Mesh mesh = new Mesh
                {
                    vertices = vertices.ToArray(), 
                    triangles = triangles.ToArray(),
                    uv = uvs.ToArray(),
                    normals = normals.ToArray()
                };
                mesh.RecalculateBounds();
                
                entity.Set(new MeshColliderInitializer(mesh, centerCoords.ToBlockCoords().ToVector3()));
                entity.Set(mesh);
                entity.Set(material);
                entity.Set(new LocalToWorld());
                entity.Set(new Translation { Value = centerCoords.ToBlockCoords().ToVector3() });
                entity.Remove<GenerateMesh>();
            }
        }

        static Vector3[] Offsets = new[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f)
        };

        void CreateQuad(Vector3 pos, BlockProperties.Properties props, BlockSide side, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector3> normals)
        {
            int tileIndex = props.TileIndex[(int)side];
            uvs.AddRange(tileIndexUVs[tileIndex]);
            
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

        const int CanvasSize = 4;
        
        void PrecalculateUVs()
        {
            for (int tileIndex = 0; tileIndex < CanvasSize * CanvasSize; tileIndex++)
            {
                tileIndexUVs[tileIndex] = GetUVs(tileIndex);
            }
        }

        Vector2[] GetUVs(int uvTileIndex)
        {
            const float BlockPixels = 16;
            const int SpacingPixels = 2;
            const float CanvasPixelSize = CanvasSize * (SpacingPixels * 2 + BlockPixels);
            const float BlockFractional = BlockPixels / CanvasPixelSize;

            int x = uvTileIndex % CanvasSize;
            float u = (SpacingPixels + x * (BlockPixels + SpacingPixels * 2)) / CanvasPixelSize;
            int y = uvTileIndex / CanvasSize;
            float v = (SpacingPixels + y * (BlockPixels + SpacingPixels * 2)) / CanvasPixelSize;

            float e = 0;//0.5f / (BlockPixels * CanvasSize);

            Vector2 uv00 = new Vector2(u + e, v + e);
            Vector2 uv10 = new Vector2(u + BlockFractional - e, v + e);
            Vector2 uv01 = new Vector2(u + e, v + BlockFractional - e);
            Vector2 uv11 = new Vector2(u + BlockFractional - e, v + BlockFractional - e);

            return new Vector2[] {uv01, uv11, uv00, uv10};
        }
    }
}