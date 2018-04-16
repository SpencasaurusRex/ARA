using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class ChunkMesh : MonoBehaviour
    {
        static Vector2[,] blockUVs;
        static ChunkMesh()
        {
            const int BLOCKS = 2;
            // How many blocks on one size
            const int CANVAS_SIZE = 8;

            blockUVs = new Vector2[BLOCKS, 4];

            // TODO: Do fancy stuff where instead of blockType as index, use a translator
            // From (blockType,side) -> index. So that if some sides of the block are the same,
            // We don't use different UVs. This approach will also help use negative numbers for blockTypes
            for (int blockType = 0; blockType < BLOCKS; blockType++)
            {
                float x = blockType % CANVAS_SIZE;
                float y = blockType / CANVAS_SIZE;
                blockUVs[blockType, 0] = new Vector2((x + 0) / CANVAS_SIZE, (y + 0) / CANVAS_SIZE);
                blockUVs[blockType, 1] = new Vector2((x + 1) / CANVAS_SIZE, (y + 0) / CANVAS_SIZE);
                blockUVs[blockType, 2] = new Vector2((x + 0) / CANVAS_SIZE, (y + 1) / CANVAS_SIZE);
                blockUVs[blockType, 3] = new Vector2((x + 1) / CANVAS_SIZE, (y + 1) / CANVAS_SIZE);
            }
        }


        public Material material;
        public ChunkCoords coords;
        public ChunkSet world;
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        List<Mesh> preMeshes = new List<Mesh>();
        List<Vector3> preLocations = new List<Vector3>();

        public void GenerateMesh()
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
                        if (type == BlockType.Air || type == BlockType.Robot) continue;
                        Vector3 position = new Vector3(x, y, z);
                        // TODO: Check for blocks across chunks, but only if the chunk is rendered!
                        if (world.GetBlockType(x + 1, y + 0, z + 0) <= 0) CreateQuad(position, Cubeside.EAST);
                        if (world.GetBlockType(x + 0, y + 0, z + 1) <= 0) CreateQuad(position, Cubeside.NORTH);
                        if (world.GetBlockType(x - 1, y + 0, z + 0) <= 0) CreateQuad(position, Cubeside.WEST);
                        if (world.GetBlockType(x + 0, y + 0, z - 1) <= 0) CreateQuad(position, Cubeside.SOUTH);
                        if (world.GetBlockType(x + 0, y + 1, z + 0) <= 0) CreateQuad(position, Cubeside.UP);
                        if (world.GetBlockType(x + 0, y - 1, z + 0) <= 0) CreateQuad(position, Cubeside.DOWN);
                    }
                }
            }

            CombineQuads();
        }

        enum Cubeside { DOWN, UP, EAST, NORTH, WEST, SOUTH };

        void CreateQuad(Vector3 pos, Cubeside side)
        {
            const int blockType = (int)BlockType.Grass;

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

            Vector2 uv00 = blockUVs[blockType, 0];
            Vector2 uv10 = blockUVs[blockType, 1];
            Vector2 uv01 = blockUVs[blockType, 2];
            Vector2 uv11 = blockUVs[blockType, 3];

            switch (side)
            {
                case Cubeside.DOWN:
                    vertices = new Vector3[] { p0, p1, p2, p3 };
                    normals = new Vector3[] {Vector3.down, Vector3.down,
                                            Vector3.down, Vector3.down};
                    break;
                case Cubeside.UP:
                    vertices = new Vector3[] { p7, p6, p5, p4 };
                    normals = new Vector3[] {Vector3.up, Vector3.up,
                                            Vector3.up, Vector3.up};
                    break;
                case Cubeside.WEST:
                    vertices = new Vector3[] { p7, p4, p0, p3 };
                    normals = new Vector3[] {Vector3.left, Vector3.left,
                                            Vector3.left, Vector3.left};
                    break;
                case Cubeside.EAST:
                    vertices = new Vector3[] { p5, p6, p2, p1 };
                    normals = new Vector3[] {Vector3.right, Vector3.right,
                                            Vector3.right, Vector3.right};
                    break;
                case Cubeside.NORTH:
                    vertices = new Vector3[] { p4, p5, p1, p0 };
                    normals = new Vector3[] {Vector3.forward, Vector3.forward,
                                            Vector3.forward, Vector3.forward};
                    break;
                case Cubeside.SOUTH:
                    vertices = new Vector3[] { p6, p7, p3, p2 };
                    normals = new Vector3[] {Vector3.back, Vector3.back,
                                            Vector3.back, Vector3.back};
                    break;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = new int[] { 3, 1, 0, 3, 2, 1 };
            mesh.uv = new Vector2[] { uv11, uv01, uv00, uv10 };

            mesh.RecalculateBounds();

            //quad.transform.position = pos;
            //quad.transform.parent = transform;

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

            // TODO: Remove this part when we optimize to not create gameObjects
            foreach (Transform quad in transform)
            {
                Destroy(quad.gameObject);
            }
        }
    }
}