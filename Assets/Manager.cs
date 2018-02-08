using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class Manager : MonoBehaviour {
        public TileObject TileEntity;
        public CameraFollow cf;

        private void Start()
        {
            #region Test setups
            // Single block
            var obj = Instantiate(TileEntity);
            MovementManager.RegisterTileEntity(obj, new Vector3Int(0, 0, 0), 15, 30);
            cf.Init(obj.transform);

            // Opposing sides
            //for (int i = 0; i < MovementManager.CHUNK_LENGTH; i++)
            //{
            //    obj = Instantiate(TileEntity);
            //    MovementManager.RegisterTileEntity(obj, new Vector3Int(i, 0, 0), i * 2 + 10, 10, 1);
            //}
            //for (int i = 0; i < MovementManager.CHUNK_LENGTH; i++)
            //{
            //    obj = Instantiate(TileEntity);
            //    MovementManager.RegisterTileEntity(obj, new Vector3Int(i, 0, MovementManager.CHUNK_LENGTH - 2), i * 3 + 5, 50, 3);
            //}

            // Matrix fill
            for (int x = 0; x < MovementManager.CHUNK_LENGTH; x++)
            {
                for (int y = 0; y < MovementManager.CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < MovementManager.CHUNK_LENGTH; z++)
                    {
                        if (x % 2 == 0 && y % 4 == 0 && z % 2 == 0)
                        {
                            obj = Instantiate(TileEntity);
                            MovementManager.RegisterTileEntity(obj, new Vector3Int(x, y, z), Random.Range(10, 100), Random.Range(10, 100), Random.Range(0, 4));
                        }
                    }
                }
            }
            #endregion
        }

        void OnDrawGizmosSelected()
        {
            foreach (var key in MovementManager.blocked.Keys)
            {
                Gizmos.color = new Color(1, 1, 0, 0.75F);
                Gizmos.DrawCube(key, Vector3.one * .99f);
            }
        }

        void FixedUpdate()
        {
            MovementManager.Tick();
            MovementManager.ControlEntities();
        }
    }
}