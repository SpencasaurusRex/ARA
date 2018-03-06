﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ARACore;

public class UnitTest {

    [Test]
    public void TestToDirection()
    {
        for (int h = 0; h < 4; h++)
        {
            Assert.AreEqual((int)Util.ToDirection(MovementAction.Forward, h), h);
        }
        
        for (int h = 0; h < 4; h++)
        {
            Assert.AreEqual((int)Util.ToDirection(MovementAction.Back, h), (h + 2) % 4);
        }

        for (int h = 0; h < 4; h++)
        {
            Assert.AreEqual(Util.ToDirection(MovementAction.Up, h), Direction.Up);
        }

        for (int h = 0; h < 4; h++)
        {
            Assert.AreEqual(Util.ToDirection(MovementAction.Down, h), Direction.Down);
        }
    }

    [Test]
    public void TestChunkSet()
    {
        ChunkSet chunks = new ChunkSet();

        // Initial BlockType is Null
        BlockType b = chunks.GetBlockType(0, 0, 0);
        Assert.AreEqual(BlockType.Null, b);

        // Can set and get blocks
        chunks.SetBlockType(45, -102, 10000, BlockType.Grass);
        b = chunks.GetBlockType(45, -102, 10000);
        Assert.AreEqual(BlockType.Grass, b);
    }

    [Test]
    public void TestChunkCoord()
    {
        // Test GetHashCode
        var cc1 = new ChunkCoords(45, 293, -792).GetHashCode();
        var cc2 = new ChunkCoords(45, 293, -792).GetHashCode();
        Assert.AreEqual(cc1, cc2);
    }

    [Test]
    public void MovementManagerPriority()
    {
        // Test all orientations with different, then same speeds

        //MovementManager.RegisterTileEntity(new TileEntity(), new Vector3Int(0, 0, 0), 10, 10, 0); // Down
        //MovementManager.RegisterTileEntity(new TileEntity(), new Vector3Int(0, 1, 0), 10, 10, 0); // Up

    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator UnitTestWithEnumeratorPasses() {
        // TODO test MovementManager
        yield return null;
	}
}
