using ARACore;
using Assets.Scripts.Chunk;
using Assets.Scripts.Movement;
using Assets.Scripts.Rendering;
using Assets.Scripts.TempMovement;
using Assets.Scripts.Transform;
using Assets.Scripts.UnityComponents;
using Boo.Lang;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Game : MonoBehaviour
    {
        public static World World;
        UpdateManager updateManager;
        RenderingSystem renderingSystem;
        TransformWriteSystem transformWriteSystem;
        MovementSlideSystem movementSlideSystem;
        SetBlockSystem setBlockSystem;
        ChunkMeshGenerationSystem chunkMeshGenerationSystem;
        List<UnityInitializer> initializationSystems;
        GameObjectCreationSystem gameObjectCreationSystem;

        public Material robotMaterial;
        public Mesh robotMesh;
        public Material chunkMaterial;
        public GameObject blankPrefab;
        public Entity global;

        void Start()
        {
            World = new World();

            global = World.CreateEntity();
            global.Set(new Global());

            updateManager = new UpdateManager
            (
                new TileEntityUpdateSystem(),
                new RobotBrainSystem(World),
                new ScriptUpdateSystem(),
                new MovementUpdateSystem(World),
                setBlockSystem = new SetBlockSystem(World)
            );

            movementSlideSystem = new MovementSlideSystem(World);
            chunkMeshGenerationSystem = new ChunkMeshGenerationSystem(World, chunkMaterial);
            transformWriteSystem = new TransformWriteSystem(World);
            renderingSystem = new RenderingSystem(World);
            initializationSystems = new List<UnityInitializer>
            {
                new MeshColliderInitializationSystem(World)
            };
            gameObjectCreationSystem = new GameObjectCreationSystem(World, blankPrefab);

            Setup();

            setBlockSystem.EndTick();
        }

        void Update()
        {
            float fractional = updateManager.Update(Time.deltaTime);
            movementSlideSystem.Update(fractional);
            chunkMeshGenerationSystem.Update();

            foreach (var initializationSystem in initializationSystems)
            {
                initializationSystem.PollForGameObjects();
            }
            gameObjectCreationSystem.Update();
            foreach (var initializationSystem in initializationSystems)
            {
                initializationSystem.Update();
            }

            transformWriteSystem.Update();
            renderingSystem.Update();
        }

        void Setup()
        {
            var props = new BlockProperties();
            
            var chunkSet = new ChunkSet(World, props);
            global.Set(chunkSet);
            global.Set(props);
            global.Set(new GameObjectMapping());

            var chunk = World.CreateEntity();
            chunkSet.GetBlock(Vector3Int.zero);
            chunkSet.GetChunkEntity(new ChunkCoords(Vector3Int.zero)).Set(new GenerateMesh());

            for (int x = 0; x < 10; x++)
                for (int y = 1; y < 10; y++)
                    for (int z = 0; z < 10; z++)
                        Robot(new Vector3Int(x, y, z));

            for (int x = -Chunk.Chunk.ChunkSize * 8; x < Chunk.Chunk.ChunkSize * 8; x++)
            {
                for (int z = -Chunk.Chunk.ChunkSize * 8; z < Chunk.Chunk.ChunkSize * 8; z++)
                {
                    for (int y = -Chunk.Chunk.ChunkSize; y < 0; y++)
                    {
                        if (y == -1)
                        {
                            chunkSet.SetBlock(new Vector3Int(x, y, z), Block.Grass);
                        }
                        else
                        {
                            chunkSet.SetBlock(new Vector3Int(x, y, z), Block.Dirt);
                        }
                    }
                }
            }

        }

        static int id;
        Entity Robot(Vector3Int initialPosition)
        {
            var entity = World.CreateEntity();
            entity.Set(robotMesh);
            entity.Set(robotMaterial);
            entity.Set(new LocalToWorld());
            entity.Set(new Translation { Value = initialPosition });
            entity.Set(new Rotation());
            entity.Set(new GridPosition { Value = initialPosition});
            entity.Set(new SetBlock {Block = Block.Robot});
            entity.Set(new Scale { Value = new Vector3(.9f, .9f, .9f)});
            entity.Set(new ID {Value = id++});

            return entity;
        }
    }
}
