using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ARACore;

public class UnitTest {

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

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator UnitTestWithEnumeratorPasses() {
        // TODO test MovementManager
        yield return null;
	}
}
