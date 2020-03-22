using Assets.Scripts.Chunk;
using Assets.Scripts.Movement;
using Assets.Scripts.Scripting;
using DefaultEcs;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class CommandTranslationSystemTests
    {
        [Test]
        public void ForwardCommandTranslatesHeading()
        {
            var (world, commandSystem) = Setup();

            // North
            var robot = world.CreateEntity();
            robot.Set(new GridPosition { Value = Vector3Int.zero });
            robot.Set(CardinalHeading.North);
            robot.Set(ScriptCommand.Forward);

            commandSystem.Update(1.0f);

            Assert.AreEqual(true, robot.Has<MovementRequest>());
            Assert.AreEqual(new Vector3Int(0, 0, 0), robot.Get<MovementRequest>().From);
            Assert.AreEqual(new Vector3Int(0, 0, 1), robot.Get<MovementRequest>().To);

            // East
            robot.Remove<MovementRequest>();
            robot.Set(CardinalHeading.East);
            commandSystem.Update(1.0f);

            Assert.AreEqual(true, robot.Has<MovementRequest>());
            Assert.AreEqual(new Vector3Int(0, 0, 0), robot.Get<MovementRequest>().From);
            Assert.AreEqual(new Vector3Int(1, 0, 0), robot.Get<MovementRequest>().To);

            // South
            robot.Remove<MovementRequest>();
            robot.Set(CardinalHeading.South);
            commandSystem.Update(1.0f);

            Assert.AreEqual(true, robot.Has<MovementRequest>());
            Assert.AreEqual(new Vector3Int(0, 0, 0), robot.Get<MovementRequest>().From);
            Assert.AreEqual(new Vector3Int(0, 0, -1), robot.Get<MovementRequest>().To);

            // West
            robot.Remove<MovementRequest>();
            robot.Set(CardinalHeading.West);
            commandSystem.Update(1.0f);

            Assert.AreEqual(true, robot.Has<MovementRequest>());
            Assert.AreEqual(new Vector3Int(0, 0, 0), robot.Get<MovementRequest>().From);
            Assert.AreEqual(new Vector3Int(-1, 0, 0), robot.Get<MovementRequest>().To);

        }

        (World, CommandTranslationSystem) Setup()
        {
            var world = new World();
            var commandSystem = new CommandTranslationSystem(world);

            return (world, commandSystem);
        }
    }
}
