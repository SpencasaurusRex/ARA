using Assets.Scripts.Chunk;
using DefaultEcs;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class ChunkSetTests
    {
        [Test]
        public void DefaultBlockIsAir()
        {
            var (world, chunkSet) = Setup();

            Block b = chunkSet.GetBlock(Vector3Int.zero);
            Assert.AreEqual(Block.Air, b);
        }

        [Test]
        public void CanSetAndGetBlock()
        {
            var (world, chunkSet) = Setup();

            var coord = new Vector3Int(45, -102, 10000);
            chunkSet.SetBlock(coord, Block.Grass);
            Block b = chunkSet.GetBlock(coord);
            Assert.AreEqual(Block.Grass, b);

            coord = new Vector3Int(-1, -1, -1);
            chunkSet.SetBlock(coord, Block.Robot);
            b = chunkSet.GetBlock(coord);
            Assert.AreEqual(Block.Robot, b);
        }

        static (World, ChunkSet) Setup()
        {
            World world = new World();
            var global = world.CreateEntity();
            var chunkSet = new ChunkSet();
            global.Set(chunkSet);

            return (world, chunkSet);
        }
    }
}