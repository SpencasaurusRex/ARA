using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class ChunkMesh : MonoBehaviour
    {
        public Material material;
        public ChunkCoords coords;
        public ChunkSet world;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        MeshCollider meshCollider;
        List<Mesh> preMeshes = new List<Mesh>();
        List<Vector3> preLocations = new List<Vector3>();
        bool markedForRegeneration;

        void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();
        }

        public void LateUpdate()
        {
            if (markedForRegeneration)
            {
                Int64 baseX = coords.cx * Chunk.CHUNK_SIZE_X;
                Int64 baseY = coords.cy * Chunk.CHUNK_SIZE_Y;
                Int64 baseZ = coords.cz * Chunk.CHUNK_SIZE_Z;

                for (Int64 z = baseZ; z < baseZ + Chunk.CHUNK_SIZE_Z; z++)
                {
                    for (Int64 y = baseY; y < baseY + Chunk.CHUNK_SIZE_Y; y++)
                    {
                        for (Int64 x = baseX; x < baseX + Chunk.CHUNK_SIZE_X; x++)
                        {
                            BlockType type = world.GetBlockType(x, y, z);
                            if (!BlockProperties.Get(type).generateMesh) continue;

                            Vector3 position = new Vector3(x, y, z);
                            // TODO: Check for blocks across chunks, but only if the chunk is rendered!
                            if (BlockProperties.Get(world.GetBlockType(x + 1, y + 0, z + 0)).transparent) CreateQuad(position, type, BlockSide.East);
                            if (BlockProperties.Get(world.GetBlockType(x + 0, y + 0, z + 1)).transparent) CreateQuad(position, type, BlockSide.North);
                            if (BlockProperties.Get(world.GetBlockType(x - 1, y + 0, z + 0)).transparent) CreateQuad(position, type, BlockSide.West);
                            if (BlockProperties.Get(world.GetBlockType(x + 0, y + 0, z - 1)).transparent) CreateQuad(position, type, BlockSide.South);
                            if (BlockProperties.Get(world.GetBlockType(x + 0, y + 1, z + 0)).transparent) CreateQuad(position, type, BlockSide.Top);
                            if (BlockProperties.Get(world.GetBlockType(x + 0, y - 1, z + 0)).transparent) CreateQuad(position, type, BlockSide.Bottom);
                        }
                    }
                }

                CombineQuads();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                markedForRegeneration = false;
            }
        }

        public void GenerateMesh()
        {
            markedForRegeneration = true;
        }

        void CreateQuad(Vector3 pos, BlockType type, BlockSide side)
        {
            int uvTileIndex = BlockProperties.Get(type).uvTileIndex[(int)side];
            Vector2[] uvs = BlockProperties.GetUVs(uvTileIndex);
            
            Mesh mesh = new Mesh();
            mesh.name = "Quad" + side.ToString();

            Vector3[] vertices = new Vector3[4];
            Vector3[] normals = new Vector3[4];

            //all possible vertices 
            Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
            Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
            Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
            Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
            Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
            Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
            Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

            switch (side)
            {
                case BlockSide.Bottom:
                    vertices = new Vector3[] { p0, p1, p2, p3 };
                    normals = new Vector3[] {Vector3.down, Vector3.down,
                                            Vector3.down, Vector3.down};
                    break;
                case BlockSide.Top:
                    vertices = new Vector3[] { p7, p6, p5, p4 };
                    normals = new Vector3[] {Vector3.up, Vector3.up,
                                            Vector3.up, Vector3.up};
                    break;
                case BlockSide.West:
                    vertices = new Vector3[] { p7, p4, p0, p3 };
                    normals = new Vector3[] {Vector3.left, Vector3.left,
                                            Vector3.left, Vector3.left};
                    break;
                case BlockSide.East:
                    vertices = new Vector3[] { p5, p6, p2, p1 };
                    normals = new Vector3[] {Vector3.right, Vector3.right,
                                            Vector3.right, Vector3.right};
                    break;
                case BlockSide.North:
                    vertices = new Vector3[] { p4, p5, p1, p0 };
                    normals = new Vector3[] {Vector3.forward, Vector3.forward,
                                            Vector3.forward, Vector3.forward};
                    break;
                case BlockSide.South:
                    vertices = new Vector3[] { p6, p7, p3, p2 };
                    normals = new Vector3[] {Vector3.back, Vector3.back,
                                            Vector3.back, Vector3.back};
                    break;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
            mesh.uv = uvs;

            mesh.RecalculateBounds();

            preLocations.Add(pos);
            preMeshes.Add(mesh);
        }

        void CombineQuads()
        {
            CombineInstance[] combine = new CombineInstance[preMeshes.Count];
            for (int i = 0; i < preMeshes.Count; i++)
            {
                combine[i].mesh = preMeshes[i];
                combine[i].transform = Matrix4x4.TRS(transform.position + preLocations[i], Quaternion.identity, Vector3.one);
            }
            preMeshes.Clear();
            preLocations.Clear();

            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.CombineMeshes(combine);

            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.material = material;
            }
        }
    }
}