using ARACore;
using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class MovementOutOfBoundsSystem : IUpdateSystem
    {
        EntitySet movementRequestSet;
        Entity globalEntity;

        public MovementOutOfBoundsSystem(World world)
        {
            movementRequestSet = world.GetEntities().With<MovementRequest>().AsSet();
            globalEntity = world.GetGlobalEntity();
        }

        public void Update(float fractional)
        {
            if (fractional != 1.0) return;

            var chunkSet = globalEntity.Get<ChunkSet>();

            foreach (var entity in movementRequestSet.GetEntities())
            {
                var request = entity.Get<MovementRequest>();
                var coords = new ChunkCoords(request.To);
                if (!chunkSet.IsChunkLoaded(coords))
                {
                    chunkSet.ChunksToLoad.Add(coords);
                }
            }
        }

        public void EndTick()
        {
        }
    }
}
