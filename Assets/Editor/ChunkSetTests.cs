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

        [Test]
        public static void ChunkEntityCreated()
        {
            var (world, chunkSet) = Setup();

            var coord = new Vector3Int(0, 0, 0);
            chunkSet.GetBlock(coord);

            var chunkEntities = world.GetEntities().With<Chunk>().AsSet().GetEntities();
            Assert.AreEqual(1, chunkEntities.Length);
        }

        [Test]
        public static void SetBlockGeneratesMesh()
        {
            var (world, chunkSet) = Setup();

            chunkSet.SetBlock(Vector3Int.zero, Block.Dirt);

            var chunkEntity = world.GetEntities().With<Chunk>().AsSet().GetEntities()[0];
            Assert.AreEqual(true, chunkEntity.Has<GenerateMesh>());
        }

        [Test]
        public static void SetBlockDoesNotGenerateMesh()
        {
            var (world, chunkSet) = Setup();

            chunkSet.SetBlock(Vector3Int.zero, Block.Robot);

            var chunkEntity = world.GetEntities().With<Chunk>().AsSet().GetEntities()[0];
            Assert.AreEqual(false, chunkEntity.Has<GenerateMesh>());
        }

        [Test]
        public static void SetBlockToSameBlockDoesNotGenerateMesh()
        {
            var (world, chunkSet) = Setup();

            chunkSet.SetBlock(Vector3Int.zero, Block.Dirt);
            var chunkEntity = world.GetEntities().With<Chunk>().AsSet().GetEntities()[0];
            chunkEntity.Remove<GenerateMesh>();

            chunkSet.SetBlock(Vector3Int.zero, Block.Dirt);
            Assert.AreEqual(false, chunkEntity.Has<GenerateMesh>());
        }

        [Test]
        public static void ChunkCoordsAreCalculatedCorrectly()
        {
            ChunkCoords coord = new ChunkCoords(new Vector3Int(Chunk.ChunkSize - 1, -1, Chunk.ChunkSize));

            Assert.AreEqual(0, coord.X);
            Assert.AreEqual(-1, coord.Y);
            Assert.AreEqual(1, coord.Z);

            coord = new ChunkCoords(new Vector3Int(Chunk.ChunkSize * 2, -Chunk.ChunkSize, -Chunk.ChunkSize - 1));

            Assert.AreEqual(2, coord.X);
            Assert.AreEqual(-1, coord.Y);
            Assert.AreEqual(-2, coord.Z);
        }

        static (World, ChunkSet) Setup()
        {
            World world = new World();
            var global = world.CreateEntity();
            var props = new BlockProperties();
            var chunkSet = new ChunkSet(world, props);
            global.Set(chunkSet);

            return (world, chunkSet);
        }
    }
}