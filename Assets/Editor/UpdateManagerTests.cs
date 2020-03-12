using System;
using System.Collections.Generic;
using ARACore;
using Assets.Scripts.Core;
using NUnit.Framework;

namespace Assets.Editor
{
    public class UpdateManagerTests
    {
        public void SingleSystem()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateManager manager = new UpdateManager(A);

            // Halfway through the tick with one system, should yield a fractional of 0.5

            manager.Update(0.5f * UpdateManager.TickLength);

            Assert.AreEqual(1, A.AdvanceTickCalls);
            Assert.AreEqual(1, A.Fractionals.Count);
            Assert.AreEqual(0.5f, A.Fractionals[0], Double.Epsilon);
        }

        [Test]
        public void SingleSystem_MultipleCalls()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateManager manager = new UpdateManager(A);

            float dt = 0.25f * UpdateManager.TickLength;

            manager.Update(dt);
            manager.Update(dt);
            manager.Update(dt);

            Assert.AreEqual(1, A.AdvanceTickCalls);
            Assert.AreEqual(3, A.Fractionals.Count);
            Assert.AreEqual(0.25f, A.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(0.50f, A.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(0.75f, A.Fractionals[2], Double.Epsilon);
        }

        [Test]
        public void SingleSystem_TickRollover()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateManager manager = new UpdateManager(A);

            float dt = 0.75f * UpdateManager.TickLength;

            manager.Update(dt);
            manager.Update(dt);

            Assert.AreEqual(2, A.AdvanceTickCalls);
            Assert.AreEqual(3, A.Fractionals.Count);
            Assert.AreEqual(0.75f, A.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, A.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(0.50f, A.Fractionals[2], Double.Epsilon);
        }

        [Test]
        public void SingleSystem_DoubleTickRollover()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateManager manager = new UpdateManager(A);

            float dt = 0.5f * UpdateManager.TickLength;
            manager.Update(dt);

            dt = 2 * UpdateManager.TickLength;
            manager.Update(dt);

            Assert.AreEqual(3, A.AdvanceTickCalls);
            Assert.AreEqual(4, A.Fractionals.Count);
            Assert.AreEqual(0.5f, A.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.0f, A.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(1.0f, A.Fractionals[2], Double.Epsilon);
            Assert.AreEqual(0.5f, A.Fractionals[3], Double.Epsilon);
        }

        [Test]
        public void MultipleSystems()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateSystem B = new UpdateSystem();
            UpdateSystem C = new UpdateSystem();

            UpdateManager manager = new UpdateManager(A, B, C);
            
            float dt = 0.5f * UpdateManager.TickLength;

            manager.Update(dt);

            Assert.AreEqual(1, A.AdvanceTickCalls);
            Assert.AreEqual(1, B.AdvanceTickCalls);
            
            Assert.AreEqual(1, A.Fractionals.Count);
            Assert.AreEqual(1, B.Fractionals.Count);

            Assert.AreEqual(1.0f, A.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(0.5f, B.Fractionals[0], Double.Epsilon);
        }

        [Test]
        public void MultipleSystems_MultipleCalls()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateSystem B = new UpdateSystem();
            UpdateSystem C = new UpdateSystem();
            UpdateManager manager = new UpdateManager(A, B, C);

            float dt = 0.25f * UpdateManager.TickLength;

            manager.Update(dt);
            manager.Update(dt);
            manager.Update(dt);

            Assert.AreEqual(1, A.AdvanceTickCalls);
            Assert.AreEqual(1, B.AdvanceTickCalls);
            Assert.AreEqual(1, C.AdvanceTickCalls);
            
            Assert.AreEqual(2, A.Fractionals.Count);
            Assert.AreEqual(2, B.Fractionals.Count);
            Assert.AreEqual(1, C.Fractionals.Count);
            
            Assert.AreEqual(0.75f, A.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, A.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(0.50f, B.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, B.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(0.25f, C.Fractionals[0], Double.Epsilon);
        }

        [Test]
        public void MultipleSystems_TickRollover()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateSystem B = new UpdateSystem();
            UpdateSystem C = new UpdateSystem();
            UpdateManager manager = new UpdateManager(A, B, C);

            float dt = 0.25f * UpdateManager.TickLength;

            manager.Update(dt);
            manager.Update(dt);
            manager.Update(dt);
            manager.Update(dt);
            manager.Update(dt);

            Assert.AreEqual(2, A.AdvanceTickCalls);
            Assert.AreEqual(2, B.AdvanceTickCalls);
            Assert.AreEqual(2, C.AdvanceTickCalls);

            Assert.AreEqual(4, A.Fractionals.Count);
            Assert.AreEqual(2, B.Fractionals.Count);
            Assert.AreEqual(2, C.Fractionals.Count);

            Assert.AreEqual(0.75f, A.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, A.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(0.50f, B.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, B.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(0.25f, C.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, C.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(0.00f, A.Fractionals[2], Double.Epsilon);
            Assert.AreEqual(0.75f, A.Fractionals[3], Double.Epsilon);
        }

        [Test]
        public void MultipleSystems_DoubleTickRollover()
        {
            UpdateSystem A = new UpdateSystem();
            UpdateSystem B = new UpdateSystem();
            UpdateSystem C = new UpdateSystem();
            UpdateManager manager = new UpdateManager(A, B, C);

            float dt = 0.25f * UpdateManager.TickLength;
            manager.Update(dt);

            dt = 2 * UpdateManager.TickLength;
            manager.Update(dt);

            Assert.AreEqual(3, A.AdvanceTickCalls);
            Assert.AreEqual(3, B.AdvanceTickCalls);
            Assert.AreEqual(3, C.AdvanceTickCalls);

            Assert.AreEqual(4, A.Fractionals.Count);
            Assert.AreEqual(2, B.Fractionals.Count);
            Assert.AreEqual(2, C.Fractionals.Count);

            Assert.AreEqual(0.75f, A.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, A.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(1.00f, B.Fractionals[0], Double.Epsilon);
            Assert.AreEqual(1.00f, C.Fractionals[0], Double.Epsilon);

            Assert.AreEqual(1.00f, A.Fractionals[2], Double.Epsilon);
            Assert.AreEqual(1.00f, B.Fractionals[1], Double.Epsilon);
            Assert.AreEqual(1.00f, C.Fractionals[1], Double.Epsilon);

            Assert.AreEqual(0.75f, A.Fractionals[3], Double.Epsilon);
        }
    }

    public class UpdateSystem : IUpdateSystem
    {
        public List<float> Fractionals = new List<float>();
        public int AdvanceTickCalls;

        public void Update(float fractional)
        {
            Fractionals.Add(fractional);
        }

        public void EndTick()
        {
            AdvanceTickCalls++;
        }
    }
}
