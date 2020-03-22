using System.Collections.Generic;
using ARACore;
using Assets.Scripts.Chunk;
using Assets.Scripts.Movement;
using Assets.Scripts.Rendering;
using Assets.Scripts.Robots;
using Assets.Scripts.Scripting;
using Assets.Scripts.Transform;
using Assets.Scripts.UnityComponents;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class Game : MonoBehaviour
    {
        World world;
        UpdateManager updateManager;
        RenderingSystem renderingSystem;
        TransformWriteSystem transformWriteSystem;
        MovementSlideSystem movementSlideSystem;
        SetBlockSystem setBlockSystem;
        ChunkMeshGenerationSystem chunkMeshGenerationSystem;
        List<UnityInitializer> initializationSystems;
        GameObjectCreationSystem gameObjectCreationSystem;
        RobotInitSystem robotInitSystem;
        TurnSystem turnSystem;

        public Material RobotMaterial;
        public Mesh RobotMesh;
        public Material ChunkMaterial;
        public GameObject BlankPrefab;
        public UnityEngine.Transform EntityBase;

        Entity global;

        void Start()
        {
            world = new World();

            global = world.CreateEntity();
            global.Set(new Global());

            updateManager = new UpdateManager
            (
                new TileEntityUpdateSystem(),
                new ScriptExecuteSystem(world),
                new CommandTranslationSystem(world),
                new MovementUpdateSystem(world),
                new TurnRequestSystem(world),
                setBlockSystem = new SetBlockSystem(world)
            );

            movementSlideSystem = new MovementSlideSystem(world);
            chunkMeshGenerationSystem = new ChunkMeshGenerationSystem(world, ChunkMaterial);
            transformWriteSystem = new TransformWriteSystem(world);
            renderingSystem = new RenderingSystem(world);
            initializationSystems = new List<UnityInitializer>
            {
                new MeshColliderInitializationSystem(world)
            };
            gameObjectCreationSystem = new GameObjectCreationSystem(world, BlankPrefab, EntityBase);
            turnSystem = new TurnSystem(world);
            robotInitSystem = new RobotInitSystem(world, RobotMesh, RobotMaterial);

            Setup();

            setBlockSystem.EndTick();
        }

        void Update()
        {
            float fractional = updateManager.Update(Time.deltaTime);
            movementSlideSystem.Update(fractional);
            turnSystem.Update(fractional);
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

            robotInitSystem.Update();

            transformWriteSystem.Update();
            renderingSystem.Update();
        }

        void Setup()
        {
            var props = new BlockProperties();
            
            var chunkSet = new ChunkSet(world, props);
            global.Set(chunkSet);
            global.Set(props);
            global.Set(new GameObjectMapping());

            var chunk = world.CreateEntity();
            chunkSet.GetBlock(Vector3Int.zero);
            chunkSet.GetChunkEntity(new ChunkCoords(Vector3Int.zero)).Set(new GenerateMesh());

            //for (int x = 0; x < 10; x++)
            //    for (int y = 1; y < 10; y++)
            //        for (int z = 0; z < 10; z++)
            //            Robot(new Vector3Int(x, y, z));

            var robot1 = Robot(Vector3Int.zero);
            robot1.Set(new ScriptInfo { Path = "script1", Status= ScriptStatus.Running});

            chunkSet.SetBlock(new Vector3Int(0, 0, 20), Block.Dirt);
            chunkSet.SetBlock(new Vector3Int(0, 0, -20), Block.Dirt);

            var robot2 = Robot(Vector3Int.one);
            robot2.Set(ScriptCommand.Forward);

            int radius = 4;

            for (int x = -Chunk.Chunk.ChunkSize * radius; x < Chunk.Chunk.ChunkSize * radius; x++)
            {
                for (int z = -Chunk.Chunk.ChunkSize * radius; z < Chunk.Chunk.ChunkSize * radius; z++)
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
            Entity entity = world.CreateEntity();
            entity.Set(new RobotInit(initialPosition));
            return entity;
        }
    }
}
