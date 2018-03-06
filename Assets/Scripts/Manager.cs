using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class Manager : MonoBehaviour {
        public TileEntity TileEntity;

        private void Start()
        {
            #region Test setups
            // Single block
            //var obj = Instantiate(TileEntity);
            //MovementManager.RegisterTileEntity(obj, new Vector3Int(0, 0, 0), 15, 30);
            //cf.Init(obj.transform);

            // Random fill
            //for (int i = 0; i < MovementManager.MAX_ENTITIES; i++)
            //{
            //    TileObject obj;
            //    int x, y, z;
            //    do
            //    {
            //        obj = Instantiate(TileEntity);
            //        x = Random.Range(0, MovementManager.CHUNK_LENGTH_X);
            //        y = Random.Range(0, MovementManager.CHUNK_HEIGHT);
            //        z = Random.Range(0, MovementManager.CHUNK_LENGTH_Z);
            //    }
            //    while (!MovementManager.RegisterTileEntity(obj, new Vector3Int(x, y, z), 100, 50));
            //}

            // Read pixel image
            //Color[] pixels = sourceTexture.GetPixels();
            //int idIndex = 0;
            //for (int i = 0; i < pixels.Length; i++)
            //{
            //    if (pixels[i].r == 0)
            //    {
            //        int x = i % sourceTexture.width;
            //        int y = MovementManager.CHUNK_HEIGHT / 2;
            //        int z = i / sourceTexture.width;
            //        MovementManager.pixelTarget[idIndex++] = new Vector3Int(x, y, z);
            //    }
            //}

            //var obj = Instantiate(TileEntity);
            //MovementManager.RegisterTileEntity(obj, new Vector3Int(0, 0, 0), 50, 50, 0);
            //// Opposing sides
            //for (int i = 0; i < MovementManager.CHUNK_LENGTH_X / 2; i++)
            //{
            //    obj = Instantiate(TileEntity);
            //    MovementManager.RegisterTileEntity(obj, new Vector3Int(i, 0, 0), 10, 10, 0);
                    
            //}
            //for (int i = 0; i < MovementManager.CHUNK_LENGTH_X; i++)
            //{
            //    var obj = Instantiate(TileEntity);
            //    MovementManager.RegisterTileEntity(obj, new Vector3Int(i, 0, MovementManager.CHUNK_LENGTH_Z - 2), i * 3 + 5, 50, 3);
            //}

            // Matrix fill
            //for (int x = 0; x < MovementManager.CHUNK_LENGTH_X; x++)
            //{
            //    for (int y = 0; y < MovementManager.CHUNK_HEIGHT; y++)
            //    {
            //        for (int z = 0; z < MovementManager.CHUNK_LENGTH_Z; z++)
            //        {
            //            if (x % 2 == 0 && y % 4 == 0 && z % 2 == 0)
            //            {
            //                var obj = Instantiate(TileEntity);
            //                MovementManager.RegisterTileEntity(obj, new Vector3Int(x, y, z), Random.Range(10, 100), Random.Range(10, 100), Random.Range(0, 4));
            //            }
            //        }
            //    }
            //}
            #endregion
        }

        //void OnDrawGizmosSelected()
        //{
        //    foreach (var key in MovementManager.blocked.Keys)
        //    {
        //        Gizmos.color = new Color(1, 1, 0, 0.75F);
        //        Gizmos.DrawCube(key, Vector3.one * .99f);
        //    }
        //}

        //void FixedUpdate()
        //{
        //    MovementManager.Tick();
        //    MovementManager.ControlEntities();
        //}
    }
}