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
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 50; y++)
                {
                    for (int z = 0; z < 50; z++)
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
            for (int x = 0; x < Chunk.CHUNK_SIZE_X * 2; x++)
            {
                for (int z = 0; z < Chunk.CHUNK_SIZE_Z * 2; z++)
                {
                    world.SetBlockType(x, -1, z, BlockType.Grass);
                }
            }

            movement = new MovementManager();

            // Single block
            //InstantiateTileEntity(TileEntity);

            // Random fill
            for (int i = 0; i < 1000; i++)
            {
                TileEntity obj;
                do
                {
                    int x = Random.Range(0, Chunk.CHUNK_SIZE_X * 2);
                    int y = Random.Range(0, Chunk.CHUNK_SIZE_Y / 2);
                    int z = Random.Range(0, Chunk.CHUNK_SIZE_Z * 2);
                    obj = Instantiate(prefab);
                    obj.transform.position = new Vector3(x, y, z);
                    obj.ticksPerTile = Random.Range(10, 50);
                    obj.ticksPerTurn = Random.Range(10, 50);
                }
                while (!RegisterWithSystems(obj));
            }

            // Side-Collision checks
            //CreateAt(new Vector3(0, 0, 0), 1, 10);
            //for (int z = 1; z < 10; z++)
            //{
            //    CreateAt(new Vector3(0, 0, z), 0, z * 15);
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

            // Conga line
            //TileEntity obj;
            //for (int i = 0; i < Chunk.CHUNK_SIZE_X; i++)
            //{
            //    obj = Instantiate(prefab);
            //    obj.transform.position = new Vector3(i, 0, 0);
            //    RegisterWithSystems(obj);
            //}

            // Opposing Sides
            //TileEntity obj;
            //for (int i = 0; i < Chunk.CHUNK_SIZE_X; i++)
            //{
            //    CreateAt(new Vector3(i, 0, 0), 1, (i + 1) * 5);
            //    CreateAt(new Vector3(i, 0, Chunk.CHUNK_SIZE_Z), 3, (i + 3) * 3);
            //}

            // Army
            //TileEntity obj;
            //for (int i = 0; i < Chunk.CHUNK_SIZE_X; i++)
            //{
            //    CreateAt(new Vector3(i, 0, -2), 1, 3);

            //    CreateAt(new Vector3(i, 0, 0), 1, 5 + i * 2);
            //    CreateAt(new Vector3(i, 0, 1), 1, 10 + i * 2);
            //    CreateAt(new Vector3(i, 0, 2), 1, 15 + i * 2);
            //    CreateAt(new Vector3(i, 0, 3), 1, 20 + i * 2);
            //    CreateAt(new Vector3(i, 0, 4), 1, 25 + i * 3);
            //}

            // Locked flower
            //CreateAt(new Vector3(0, 0, 0), 0, 5);
            //CreateAt(new Vector3(1, 0, 0), 1, 5);
            //CreateAt(new Vector3(1, 0, 1), 2, 5);
            //CreateAt(new Vector3(0, 0, 1), 3, 5);

            // Lel wat
            //CreateAt(new Vector3(0, 0, 0), 0, 2, 5);
            //CreateAt(new Vector3(1, 0, 0), 1, 5, 5);
            //CreateAt(new Vector3(1, 0, 1), 2, 5, 5);

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
        void FixedUpdate()
        {
            movement.Tick();
            foreach (var robot in tileEntities)
            {
                //movement.RequestMovement(robot.id, MovementAction.Forward);

                var movementType = (MovementAction)Random.Range(0, 6);
                //if (movementType == MovementAction.Down) continue;
                //if (movementType == MovementAction.Up) continue;
                //if (movementType == MovementAction.TurnLeft) continue;
                //if (movementType == MovementAction.TurnRight) continue;
                //if (movementType == MovementAction.Forward) continue;
                //if (movementType == MovementAction.Back) continue;
                movement.RequestMovement(robot.id, movementType);
            }
        }

        private void CreateAt(Vector3 pos, int heading, int ticksPerTile = 50, int ticksPerTurn = 50)
        {
            TileEntity obj = Instantiate(prefab);
            obj.transform.position = pos;
            obj.transform.rotation = Util.ToQuaternion(heading);
            obj.startHeading = heading;
            obj.ticksPerTile = ticksPerTile;
            obj.ticksPerTurn = ticksPerTurn;
            RegisterWithSystems(obj);
        }

        // TODO Make this deterministic if two entities are being created onto the same tile
        private bool RegisterWithSystems(TileEntity entity)
        {
            // Set ID
            entity.id = id++;

            // ChunkSet stuff
            Vector3Int tileLocation = Vector3Int.FloorToInt(entity.transform.position);
            if (world.IsAir(tileLocation))
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