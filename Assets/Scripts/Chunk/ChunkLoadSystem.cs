using ARACore;
using Assets.Scripts.Core;
using DefaultEcs;

namespace Assets.Scripts.Chunk
{
    public class ChunkLoadSystem : IUpdateSystem
    {
        Entity globalEntity;

        public ChunkLoadSystem(World world)
        {
            globalEntity = world.GetGlobalEntity();
        }

        public void Update(float fractional)
        {
            if (fractional != 1.0f) return;

            var chunkSet = globalEntity.Get<ChunkSet>();

            foreach (var chunkCoord in chunkSet.ChunksToLoad)
            {
                LoadChunk(chunkCoord);
            }
        }

        void LoadChunk(ChunkCoords coords)
        {
            var chunkSet = globalEntity.Get<ChunkSet>();
            chunkSet.GetBlock(coords.ToBlockCoords());
        }

        public void EndTick()
        {
        }
    }
}
