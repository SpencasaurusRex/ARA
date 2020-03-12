using Assets.Scripts.Core;
using DefaultEcs;

namespace Assets.Scripts.Chunk
{
    public class SetBlockSystem : IUpdateSystem
    {
        EntitySet setBlockSet;
        EntitySet removeBlockSet;
        EntitySet globalEntitySet;

        public SetBlockSystem(World world)
        {
            setBlockSet = world.GetEntities().With<GridPosition>().With<SetBlock>().AsSet();
            removeBlockSet = world.GetEntities().With<RemoveBlock>().AsSet();
            globalEntitySet = world.GetEntities().With<Global>().AsSet();
        }

        public void Update(float fractional)
        {
        }

        public void EndTick()
        {
            var chunkSet = globalEntitySet.GetEntities()[0].Get<ChunkSet>();

            foreach (var entity in removeBlockSet.GetEntities())
            {
                var position = entity.Get<RemoveBlock>().Coords;
                chunkSet.SetBlock(position, Block.Air);
                entity.Remove<RemoveBlock>();
            }

            foreach (var entity in setBlockSet.GetEntities())
            {
                var block = entity.Get<SetBlock>().Block;
                var position = entity.Get<GridPosition>().Value;
                chunkSet.SetBlock(position, block);
                entity.Remove<SetBlock>();
            }
        }
    }
}
