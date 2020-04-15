using ARACore;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Chunk
{
    public class CameraLoadRadiusSystem
    {
        const int LoadRadius = 5;
        const int UnloadRadius = 8;

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

            for (int x = cameraChunkCoords.X - LoadRadius - 1; x <= cameraChunkCoords.X + LoadRadius + 1; x++)
            {
                for (int y = cameraChunkCoords.Y - LoadRadius - 1; y <= cameraChunkCoords.Y + LoadRadius + 1; y++)
                {
                    for (int z = cameraChunkCoords.Z - LoadRadius - 1; z <= cameraChunkCoords.Z + LoadRadius + 1; z++)
                    {
                        var coords = new ChunkCoords(x, y, z);

                        if ((cameraPosition.position - coords.CenterCoords).magnitude > LoadRadius * Chunk.ChunkSize) continue;
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
                if ((chunkCoord.CenterCoords - cameraPosition.position).magnitude > UnloadRadius * Chunk.ChunkSize)
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
