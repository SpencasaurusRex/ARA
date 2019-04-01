using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class Manager : MonoBehaviour {
        public TileEntity robotPrefab;
        public Material BlockMaterial;
        public static MovementManager movement;
        public static ChunkSet world;
        public static IdManager robotManager;

        ScriptManager scriptManager;
        List<TileEntity> tileEntities = new List<TileEntity>();

        void Awake()
        {
            BlockProperties.ReadJson();
            scriptManager = new ScriptManager();
        }

        #region Test setups
        void SingleBlock()
        {
            ulong id;
            if (CreateAt(Vector3.zero, 1, out id, 30, 30))
            {
                Camera.main.GetComponent<ThirdPersonCamera>().Focus(robotManager.Get(id).tileEntity.transform);
            }
        }

        void Friend()
        {
            ulong id;
            CreateAt(Vector3.one, 0, out id, 10);
        }

        void FullFill()
        {
            for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
            {
                for (int z = 0; z < Chunk.CHUNK_SIZE_Z; z++)
                {
                    ulong id;
                    CreateAt(new Vector3(x, 0, z), 1, out id);
                }
            }
        }

        void Lattice()
        {
            for (int x = 0; x < Chunk.CHUNK_SIZE_X; x += 2)
            {
                for (int z = 0; z < Chunk.CHUNK_SIZE_Z; z += 2)
                {
                    ulong id;
                    CreateAt(new Vector3(x, 0, z), 1, out id);
                }
            }
        }

        void RandomFill()
        {
            ulong id;
            for (int i = 0; i < 1000;)
            {
                int x = Random.Range(0, Chunk.CHUNK_SIZE_X * 2);
                int y = Random.Range(0, Chunk.CHUNK_SIZE_Y / 2);
                int z = Random.Range(0, Chunk.CHUNK_SIZE_Z * 2);

                if (CreateAt(new Vector3(x, y, z), Random.Range(0, 4), out id, Random.Range(10, 50), Random.Range(10, 50)))
                {
                    i++;
                }
            }
        }

        void SideCollision()
        {
            ulong id;
            CreateAt(new Vector3(0, 0, 0), 1, out id, 10);
            for (int z = 1; z < 10; z++)
            {
                CreateAt(new Vector3(0, 0, z), 0, out id, z * 15);
            }
        }

        void CongaLine()
        {
            ulong id;
            for (int i = 0; i < Chunk.CHUNK_SIZE_X; i++)
            {
                CreateAt(new Vector3(i, 0, 0), 0, out id);
            }
        }

        void OpposingSides()
        {
            ulong id;
            for (int i = 0; i < Chunk.CHUNK_SIZE_X; i++)
            {
                CreateAt(new Vector3(i, 0, 0), 1, out id, (i + 1) * 5);
                CreateAt(new Vector3(i, 0, Chunk.CHUNK_SIZE_Z), 3, out id, (i + 3) * 3);
            }
        }

        void Army()
        {
            ulong id;
            for (int i = 0; i < 40; i++)
            {
                CreateAt(new Vector3(i, 0, -2), 1, out id, 3);
                for (int j = 1; j < 40; j++)
                {
                    CreateAt(new Vector3(i, 0, j - 1), 1, out id, (j + i) / 2 + 20, (j + i) / 2 + 20);
                }
            }
        }

        void ContinousArmy()
        {
            ulong id;
            for (int i = 0; i < Chunk.CHUNK_SIZE_X; i++)
            {
                CreateAt(new Vector3(i, 0, -2), 1, out id);

                CreateAt(new Vector3(i, 0, 0), 1, out id);
                CreateAt(new Vector3(i, 0, 1), 1, out id);
                CreateAt(new Vector3(i, 0, 2), 1, out id);
                CreateAt(new Vector3(i, 0, 3), 1, out id);
                CreateAt(new Vector3(i, 0, 4), 1, out id);
            }
        }

        void ClashingArmies()
        {
            ulong id;
            for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
            {
                for (int z = 0; z < 5; z++)
                {
                    CreateAt(new Vector3(x, 0, z), 1, out id, 30, 30);
                }
            }
            for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
            {
                for (int z = 0; z < 5; z++)
                {
                    CreateAt(new Vector3(x, 0, Chunk.CHUNK_SIZE_Z * 2 - z), 3, out id, 20, 20);
                }
            }
            for (int x = 0; x < 5; x++)
            {
                for (int z = Chunk.CHUNK_SIZE_Z / 2; z < Chunk.CHUNK_SIZE_Z * 1.5f; z++)
                {
                    CreateAt(new Vector3(x - 6, 0, z), 0, out id, 15, 15);
                }
            }
            for (int x = 0; x < 5; x++)
            {
                for (int z = Chunk.CHUNK_SIZE_Z / 2; z < Chunk.CHUNK_SIZE_Z * 1.5f; z++)
                {
                    CreateAt(new Vector3(Chunk.CHUNK_SIZE_X - x + 5, 0, z), 2, out id, 13, 13);
                }
            }
        }

        void LockedFlower()
        {
            ulong id;
            CreateAt(new Vector3(0, 0, 0), 0, out id);
            CreateAt(new Vector3(1, 0, 0), 1, out id);
            CreateAt(new Vector3(1, 0, 1), 2, out id);
            CreateAt(new Vector3(0, 0, 1), 3, out id);
        }

        void MatrixFill()
        {
            ulong id;
            for (int x = 0; x < Chunk.CHUNK_SIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNK_SIZE_Y; y++)
                {
                    for (int z = 0; z < Chunk.CHUNK_SIZE_Z; z++)
                    {
                        if (x % 2 == 0 && y % 4 == 0 && z % 2 == 0)
                        {
                            CreateAt(new Vector3(x, y, z), Random.Range(0, 4), out id, Random.Range(10, 100), Random.Range(10, 100));
                        }
                    }
                }
            }
        }

        void Line()
        {
            ulong id;
            for (int i = 0; i < 10; i++)
            {
                CreateAt(new Vector3(i, 0, 0), 1, out id);
            }
        }

        void Cube()
        {
            ulong id = 0;
            const int cubeSize = 12;
            for (int x = 0; x < cubeSize; x++)
            {
                for (int y = 0; y < cubeSize / 2; y++)
                {
                    for (int z = 0; z < cubeSize; z++)
                    {
                        int ticksPerTile = (35 - (int)(id / (cubeSize * cubeSize / 2))) / 2;
                        CreateAt(new Vector3(x, y, z), 0, out id, ticksPerTile, ticksPerTile / 2);
                    }
                }
            }
        }
        #endregion Test setups

        void Start()
        {
            Chunk.BlockMaterial = BlockMaterial;
            world = GetComponent<ChunkSet>();
            world.GenerateWorld();
            movement = new MovementManager();
            robotManager = new IdManager();

            //ContinousArmy();
            //ClashingArmies();
            Cube();
            //Line();
        }
        void FixedUpdate()
        {
            movement.Tick();
            foreach (var robot in tileEntities)
            {
                if (!movement.IsMoving(robot.Id))
                {
                    int result = scriptManager.Run(robot.scriptId);
                    if (result < 0)
                    {
                        continue;
                    }
                    movement.RequestMovement(robot.Id, (MovementAction)result);
                }
            }
        }

        void OnDrawGizmos()
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

            movement.DrawMoveResults();
        }

        bool CreateAt(Vector3 pos, int heading, out ulong id, int ticksPerTile = 50, int ticksPerTurn = 50)
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
        bool RegisterWithSystems(TileEntity entity, out ulong id)
        {
            // Set ID
            id = robotManager.Assign(entity.gameObject);

            // ChunkSet stuff
            Vector3Int tileLocation = Vector3Int.FloorToInt(entity.transform.position);
            if (world.IsAir(tileLocation.x, tileLocation.y, tileLocation.z))
            {
                world.CreateBlock(tileLocation.x, tileLocation.y, tileLocation.z, BlockType.Robot);
            }
            else
            {
                Destroy(entity.gameObject);
                robotManager.Unassign(id);
                return false;
            }

            // Movement stuff
            movement.RegisterTileEntity(entity);

            // TODO: Move script instantiation
            ulong scriptId = scriptManager.CreateScript(id);
            entity.scriptId = scriptId;

            tileEntities.Add(entity);
            return true;
        }
    }
}