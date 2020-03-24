using ARACore;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public class CameraLoadRadiusSystem
    {
        const int LoadRadius = 4;
        const int UnloadRadius = 6;

        Entity globalEntity;
        UnityEngine.Transform cameraPosition;
        EntitySet chunkMeshSet;

        public CameraLoadRadiusSystem(World world, UnityEngine.Transform cameraPosition)
        {
            globalEntity = world.GetGlobalEntity();
            this.cameraPosition = cameraPosition;
            chunkMeshSet = world.GetEntities().With<Chunk>().With<Mesh>().AsSet();
        }

        public void Update()
        {
            var chunkSet = globalEntity.Get<ChunkSet>();
            var cameraChunkCoords = new ChunkCoords(Vector3Int.RoundToInt(cameraPosition.position));

            for (int x = cameraChunkCoords.X - LoadRadius; x <= cameraChunkCoords.X + LoadRadius; x++)
            {
                for (int y = cameraChunkCoords.Y - LoadRadius; y <= cameraChunkCoords.Y + LoadRadius; y++)
                {
                    for (int z = cameraChunkCoords.Z - LoadRadius; z <= cameraChunkCoords.Z + LoadRadius; z++)
                    {
                        var coords = new ChunkCoords(x, y, z);
                        var chunkEntity = chunkSet.GetChunkEntity(coords);
                        if (!chunkEntity.Has<Mesh>())
                        {
                            chunkEntity.Set(new GenerateMesh { Coords = coords });
                        }
                    }
                }
            }

            foreach (var chunkEntity in chunkMeshSet.GetEntities())
            {
                var chunk = chunkEntity.Get<Chunk>();
                var chunkCoord = chunk.Coord;
                if (Mathf.Abs(cameraChunkCoords.X - chunkCoord.X) >= UnloadRadius ||
                    Mathf.Abs(cameraChunkCoords.Y - chunkCoord.Y) >= UnloadRadius ||
                    Mathf.Abs(cameraChunkCoords.Z - chunkCoord.Z) >= UnloadRadius)
                {
                    if (chunk.TileEntities.Count == 0)
                    {
                        // Unload logically, nothing running in the chunk
                        chunkSet.UnloadChunkLogically(chunkCoord);
                    }

                    chunkEntity.Remove<Mesh>();
                }
            }
        }
    }
}
