using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class Manager : MonoBehaviour {
        public TileEntity prefab;
        private List<TileEntity> tileEntities = new List<TileEntity>();
        public static MovementManager movement;
        public static ChunkSet world;
        public static ulong id;

        void OnDrawGizmos()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        if (world != null)
                        {
                            if (world.GetBlockType(x, y, z) == BlockType.Air)
                            {
                                continue;
                            }
                        }
                        Gizmos.DrawCube(new Vector3(x, y, z), new Vector3(.5f, .5f, .5f));
                    }
                }
            }
        }

        private void Start()
        {
            #region Test setups

            world = new ChunkSet();
            movement = new MovementManager();

            // Single block
            //InstantiateTileEntity(TileEntity);

            // Random fill
            for (int i = 0; i < 1; i++)
            {
                TileEntity obj;
                do
                {
                    int x = 0;// Random.Range(0, Chunk.CHUNK_SIZE_X);
                    int y = 0;// Random.Range(0, Chunk.CHUNK_SIZE_Y);
                    int z = 0;// Random.Range(0, Chunk.CHUNK_SIZE_Z);
                    obj = Instantiate(prefab);
                    obj.transform.position = new Vector3(x, y, z);
                }
                while (!RegisterWithSystems(obj));
            }

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

        void FixedUpdate()
        {
            movement.Tick();
            foreach (var robot in tileEntities)
            {
                movement.RequestMovement(robot.id, MovementAction.Forward);
            }
        }

        // TODO Make this deterministic if two entities are being created onto the same tile
        private bool RegisterWithSystems(TileEntity entity)
        {
            // Set ID
            entity.id = id++;

            // ChunkSet stuff
            Vector3Int tileLocation = Vector3Int.FloorToInt(entity.transform.position);
            if (world.IsAir(tileLocation.x, tileLocation.y, tileLocation.z))
            {
                world.SetBlockType(tileLocation.x, tileLocation.y, tileLocation.z, BlockType.Robot);
            }
            else
            {
                id--;
                Destroy(entity.gameObject);
                return false;
            }

            // Movement stuff
            movement.RegisterTileEntity(entity);

            tileEntities.Add(entity);
            return true;
        }
    }
}