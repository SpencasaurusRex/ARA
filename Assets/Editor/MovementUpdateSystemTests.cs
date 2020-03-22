using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using Assets.Scripts.Movement;
using DefaultEcs;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class MovementUpdateSystemTests
    {
        [Test]
        public void SingleRobotCanMove()
        {
            var (world, system, chunkSet) = Setup();
            var entity = world.CreateEntity();
            entity.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));

            system.Update(1);
            system.EndTick();

            var result = entity.Get<ActionResult>().Result;

            Assert.AreEqual(true, result);
        }

        [Test]
        public void RobotCannotCollideWithWorld()
        {
            var (world, system, chunkSet) = Setup();
            var entity = world.CreateEntity();

            var destination = new Vector3Int(0, 0, 1);

            entity.Set(new MovementRequest(new Vector3Int(0, 0, 0), destination));

            chunkSet.SetBlock(destination, Block.Dirt);

            system.Update(1);
            system.EndTick();

            var result = entity.Get<ActionResult>().Result;
        }

        [Test]
        public void MovedRobotUpdatesWorld()
        {
            var (world, system, chunkSet) = Setup();
            var entity = world.CreateEntity();

            var startPosition = Vector3Int.zero;
            entity.Set(new MovementRequest(startPosition, new Vector3Int(0, 0, 1)));

            system.Update(1);
            system.EndTick();

            Assert.AreEqual(true, entity.Has<SetBlock>());
            Assert.AreEqual(Block.Robot, entity.Get<SetBlock>().Block);
            Assert.AreEqual(true, entity.Has<RemoveBlock>());
            Assert.AreEqual(startPosition, entity.Get<RemoveBlock>().Coords);
        }

        [Test]
        public void BlockedRobotDoesNotUpdateWorld()
        {
            var (world, system, chunkSet) = Setup();
            var entity = world.CreateEntity();

            var destination = new Vector3Int(0, 0, 1);

            entity.Set(new MovementRequest(new Vector3Int(0, 0, 0), destination));

            chunkSet.SetBlock(destination, Block.Dirt);

            system.Update(1);
            system.EndTick();

            Assert.AreEqual(false, entity.Has<SetBlock>());
            Assert.AreEqual(false, entity.Has<RemoveBlock>());
        }

        [Test]
        public void CollidingRobotsCantMove()
        {
            var (world, system, chunkSet) = Setup();
            var A = world.CreateEntity();
            A.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));
            var B = world.CreateEntity();
            B.Set(new MovementRequest(new Vector3Int(0, 0, 2), new Vector3Int(0, 0, 1)));

            system.Update(1);
            system.EndTick();

            var resultA = A.Get<ActionResult>().Result;
            var resultB = B.Get<ActionResult>().Result;

            Assert.AreEqual(false, resultA);
            Assert.AreEqual(false, resultB);
        }

        [Test]
        public void ChainedRobotsCanMove()
        {
            var (world, system, chunkSet) = Setup();
            var A = world.CreateEntity();
            A.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));
            var B = world.CreateEntity();
            B.Set(new MovementRequest(new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 2)));

            system.Update(1);
            system.EndTick();

            var resultA = A.Get<ActionResult>().Result;
            var resultB = B.Get<ActionResult>().Result;

            Assert.AreEqual(true, resultA);
            Assert.AreEqual(true, resultB);
        }

        [Test]
        public void SideCollisionsCannotMove()
        {
            var (world, system, chunkSet) = Setup();
            var A = world.CreateEntity();
            A.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));
            var B = world.CreateEntity();
            B.Set(new MovementRequest(new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0)));

            system.Update(1);
            system.EndTick();

            var resultA = A.Get<ActionResult>().Result;
            var resultB = B.Get<ActionResult>().Result;

            Assert.AreEqual(false, resultA);
            Assert.AreEqual(true, resultB);
        }

        static (World, MovementUpdateSystem, ChunkSet) Setup()
        {
            World world = new World();
            BlockProperties blockProperties = new BlockProperties();
            MovementUpdateSystem system = new MovementUpdateSystem(world);

            var chunkSet = new ChunkSet(world, blockProperties);
            var global = world.CreateEntity();
            global.Set(new Global());
            global.Set(chunkSet);

            return (world, system, chunkSet);
        }
    }
}
