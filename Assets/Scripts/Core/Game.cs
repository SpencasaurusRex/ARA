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
        CameraLoadRadiusSystem cameraLoadRadiusSystem;
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
        public UnityEngine.Transform Camera;

        Entity global;

        void Start()
        {
            world = new World();

            global = world.CreateEntity();
            global.Set(new Global());

            updateManager = new UpdateManager
            (
                new TileEntityUpdateSystem(),
                //new RobotBrainSystem(world),
                new ScriptExecuteSystem(world),
                new CommandTranslationSystem(world),
                new MovementOutOfBoundsSystem(world),
                new ChunkLoadSystem(world),
                new MovementUpdateSystem(world),
                new TurnRequestSystem(world),
                setBlockSystem = new SetBlockSystem(world)
            );

            movementSlideSystem = new MovementSlideSystem(world);
            cameraLoadRadiusSystem = new CameraLoadRadiusSystem(world, Camera);
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
            cameraLoadRadiusSystem.Update();
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

            //for (int x = 0; x < 10; x++)
            //for (int y = 1; y < 10; y++)
            //for (int z = 0; z < 10; z++)
            //{
            //    var robot = Robot(new Vector3Int(x, y, z));
            //    robot.Set(new ScriptInfo { Path="script1", Status = ScriptStatus.Running});
            //}

            //for (int x = 0; x < 10; x++)
            //for (int y = 1; y < 10; y++)
            //{
            //    chunkSet.SetBlock(new Vector3Int(x, y, 20), Block.Dirt);
            //    chunkSet.SetBlock(new Vector3Int(x, y, -20), Block.Dirt);
            //}

            var robot1 = Robot(new Vector3Int(0, -1, 0));
            robot1.Set(new ScriptInfo { Path = "script1", Status = ScriptStatus.Running });

            //chunkSet.SetBlock(new Vector3Int(0, 0, 20), Block.Dirt);
            //chunkSet.SetBlock(new Vector3Int(0, 0, -20), Block.Dirt);

            //var robot2 = Robot(Vector3Int.one);
            //robot2.Set(ScriptCommand.Forward);
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
