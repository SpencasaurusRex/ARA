using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using UnityEngine;

public class BlockPacer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PlaceBlocks());
    }

    IEnumerator PlaceBlocks()
    {
        for (int z = 0; z < Chunk.ChunkSize; z++)
        {
            for (int y = 0; y < Chunk.ChunkSize; y++)
            {
                for (int x = 0; x < Chunk.ChunkSize; x++)
                {
                    Game.World.Get<ChunkSet>()[0].SetBlock(new Vector3Int(x, y, z), Block.Grass);
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }
}
