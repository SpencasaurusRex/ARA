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
            var (world, system) = Setup();
            var entity = world.CreateEntity();
            entity.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));

            system.Update(1);

            var result = entity.Get<MovementResult>().Result;

            Assert.AreEqual(true, result);
        }

        [Test]
        public void CollidingRobotsCantMove()
        {
            var (world, system) = Setup();
            var A = world.CreateEntity();
            A.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));
            var B = world.CreateEntity();
            B.Set(new MovementRequest(new Vector3Int(0, 0, 2), new Vector3Int(0, 0, 1)));

            system.Update(1);

            var resultA = A.Get<MovementResult>().Result;
            var resultB = B.Get<MovementResult>().Result;

            Assert.AreEqual(false, resultA);
            Assert.AreEqual(false, resultB);
        }

        [Test]
        public void ChainedRobotsCanMove()
        {
            var (world, system) = Setup();
            var A = world.CreateEntity();
            A.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));
            var B = world.CreateEntity();
            B.Set(new MovementRequest(new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 2)));

            system.Update(1);

            var resultA = A.Get<MovementResult>().Result;
            var resultB = B.Get<MovementResult>().Result;

            Assert.AreEqual(true, resultA);
            Assert.AreEqual(true, resultB);
        }

        [Test]
        public void SideCollisionsCannotMove()
        {
            var (world, system) = Setup();
            var A = world.CreateEntity();
            A.Set(new MovementRequest(new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1)));
            var B = world.CreateEntity();
            B.Set(new MovementRequest(new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0)));

            system.Update(1);

            var resultA = A.Get<MovementResult>().Result;
            var resultB = B.Get<MovementResult>().Result;

            Assert.AreEqual(false, resultA);
            Assert.AreEqual(false, resultB);
        }

        static (World, MovementUpdateSystem) Setup()
        {
            World world = new World();
            MovementUpdateSystem system = new MovementUpdateSystem(world);

            //var global = world.CreateEntity();
            //global.Set(new Global());

            return (world, system);
        }
    }
}
