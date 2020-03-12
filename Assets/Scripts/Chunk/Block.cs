using System.Linq;

namespace Assets.Scripts.Chunk
{
    public enum Block
    {
        Air = 0,
        Stone,
        Grass,
        Dirt,
        Robot,
        Furnace
    }

    public static class BlockProperties
    {
        static Block[] TileEntities =
        {
            Block.Robot, Block.Furnace
        };

        static bool IsTileEntity(this Block block) => TileEntities.Contains(block);
    }
}