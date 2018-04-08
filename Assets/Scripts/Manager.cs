using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class Manager : MonoBehaviour {
        public TileEntity robotPrefab;
        public Material BlockMaterial;

        private List<TileEntity> tileEntities = new List<TileEntity>();
        public static MovementManager movement;
        public static ChunkSet world;

        private void Start()
        {
            Chunk.BlockMaterial = BlockMaterial;
            world = new ChunkSet();
            movement = new MovementManager();

            #region Test setups
            // Single block..
            ulong id;
            if (CreateAt(Vector3.zero, 1, out id, 100))
            {
                Camera.main.GetComponent<ThirdPersonCamera>().Focus(IdManager.Get(id).transform); 
            }
            // ..And his friend
            CreateAt(Vector3.one, 0, out id, 10);

            // Random fill
            //for (int i = 0; i < 1000; i++)
            //{
            //    TileEntity obj;
            //    do
            //    {
            //        int x = Random.Range(0, Chunk.CHUNK_SIZE_X * 2);
            //        int y = Random.Range(0, Chunk.CHUNK_SIZE_Y / 2);
            //        int z = Random.Range(0, Chunk.CHUNK_SIZE_Z * 2);
            //        obj = Instantiate(prefab);
            //        obj.transform.position = new Vector3(x, y, z);
            //        obj.ticksPerTile = Random.Range(10, 50);
            //        obj.ticksPerTurn = Random.Range(10, 50);
            //    }
            //    while (!RegisterWithSystems(obj));
            //}

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
            //for (int i = 0; i < Chunk.CHUNK_SIZE_X * 2; i++)
            //{
            //    CreateAt(new Vector3(i, 0, -2), 1, 3);

            //    for (int j = 1; j < 20; j++)
            //    {
            //        CreateAt(new Vector3(i, 0, j - 1), 1, j + i + 20);
            //    }
            //}

            // Continuous army
            //for (int i = 0; i < Chunk.CHUNK_SIZE_X; i++)
            //{
            //    CreateAt(new Vector3(i, 0, -2), 1);

            //    CreateAt(new Vector3(i, 0, 0), 1);
            //    CreateAt(new Vector3(i, 0, 1), 1);
            //    CreateAt(new Vector3(i, 0, 2), 1);
            //    CreateAt(new Vector3(i, 0, 3), 1);
            //    CreateAt(new Vector3(i, 0, 4), 1);
            //}

            // Clashing armies
            //for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
            //{
            //    for (int z = 0; z < 5; z++)
            //    {
            //        CreateAt(new Vector3(x, 0, z), 1);
            //    }
            //}
            //for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
            //{
            //    for (int z = 0; z < 5; z++)
            //    {
            //        CreateAt(new Vector3(x, 0, Chunk.CHUNK_SIZE_Z * 2 - z), 3);
            //    }
            //}
            //for (int x = 0; x < 5; x++)
            //{
            //    for (int z = Chunk.CHUNK_SIZE_Z / 2; z < Chunk.CHUNK_SIZE_Z * 1.5f; z++)
            //    {
            //        CreateAt(new Vector3(x - 5, 0, z), 0, 60);
            //    }
            //}
            //for (int x = 0; x < 5; x++)
            //{
            //    for (int z = Chunk.CHUNK_SIZE_Z / 2; z < Chunk.CHUNK_SIZE_Z * 1.5f; z++)
            //    {
            //        CreateAt(new Vector3(Chunk.CHUNK_SIZE_X - x + 5, 0, z), 2, 60 - x);
            //    }
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
                movement.RequestMovement(robot.Id, MovementAction.Forward);

                //var movementType = (MovementAction)Random.Range(0, 6);
                //if (movementType == MovementAction.Down) continue;
                //if (movementType == MovementAction.Up) continue;
                //if (movementType == MovementAction.TurnLeft) continue;
                //if (movementType == MovementAction.TurnRight) continue;
                //if (movementType == MovementAction.Forward) continue;
                //if (movementType == MovementAction.Back) continue;
                //movement.RequestMovement(robot.id, movementType);
            }
        }

        private void OnDrawGizmos()
        {
            if (movement == null || movement.forwardChecks == null) return;
            Gizmos.color = Color.white;
            foreach (var forwardCheck in movement.forwardChecks)
            {
                var start = forwardCheck.Key.tilePosition;
                var dir = Util.ToVector3Int(forwardCheck.Key.direction);
                var end = start + dir;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawCube(end, Vector3.one * 0.25f);
            }
            Gizmos.color = Color.red;
            foreach (var forwardCheck in movement.blockedMoves)
            {
                var start = forwardCheck.Key.tilePosition;
                var dir = Util.ToVector3Int(forwardCheck.Key.direction);
                var end = start;
                start = start - dir;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawCube(end, Vector3.one * 0.25f);
            }

            //if (world == null) return;
            //Gizmos.color = Color.red;
            //for (int x = 0; x < 50; x++)
            //{
            //    for (int y = 0; y < 50; y++)
            //    {
            //        for (int z = 0; z < 50; z++)
            //        {
            //            if (!world.IsAir(x, y, z))
            //            {
            //                Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one * 0.25f);
            //            }
            //        }
            //    }
            //}
        }

        private bool CreateAt(Vector3 pos, int heading, out ulong id, int ticksPerTile = 50, int ticksPerTurn = 50)
        {
            TileEntity obj = Instantiate(robotPrefab);
            obj.transform.position = pos;
            obj.transform.rotation = Util.ToQuaternion(heading);
            obj.StartHeading = heading;
            obj.TicksPerTile = ticksPerTile;
            obj.TicksPerTurn = ticksPerTurn;
            return RegisterWithSystems(obj, out id);
        }

        // TODO Make this deterministic if two entities are being created onto the same tile
        private bool RegisterWithSystems(TileEntity entity, out ulong id)
        {
            // Set ID
            id = IdManager.Assign(entity.gameObject);

            // ChunkSet stuff
            Vector3Int tileLocation = Vector3Int.FloorToInt(entity.transform.position);
            if (world.IsAir(tileLocation))
            {
                world.CreateBlock(tileLocation.x, tileLocation.y, tileLocation.z, BlockType.Robot);
            }
            else
            {
                Destroy(entity.gameObject);
                IdManager.Unassign(id);
                return false;
            }

            // Movement stuff
            movement.RegisterTileEntity(entity);

            tileEntities.Add(entity);
            return true;
        }
    }
}