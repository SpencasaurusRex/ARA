using ARACore;
using Assets.Scripts.Chunk;
using Assets.Scripts.Movement;
using Assets.Scripts.Rendering;
using Assets.Scripts.TempMovement;
using Assets.Scripts.Transform;
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

        public Material robotMaterial;
        public Mesh robotMesh;
        public Material chunkMaterial;

        void Start()
        {
            World = new World();

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

            Setup();

            setBlockSystem.EndTick();
        }

        void Update()
        {
            float fractional = updateManager.Update(Time.deltaTime);
            movementSlideSystem.Update(fractional);
            chunkMeshGenerationSystem.Update();
            transformWriteSystem.Update();
            renderingSystem.Update();
        }

        void Setup()
        {
            var props = new BlockProperties();
            
            var global = World.CreateEntity();
            var chunkSet = new ChunkSet(World, props);
            global.Set(new Global());
            global.Set(chunkSet);
            global.Set(props);

            var chunk = World.CreateEntity();
            chunkSet.GetBlock(Vector3Int.zero);
            chunkSet.GetChunkEntity(new ChunkCoords(Vector3Int.zero)).Set(new GenerateMesh());

            for (int x = 0; x < 10; x++)
            for (int y = 1; y < 10; y++)
            for (int z = 0; z < 10; z++)
                Robot(new Vector3Int(x, y, z));

            for (int x = -Chunk.Chunk.ChunkSize * 4; x < Chunk.Chunk.ChunkSize * 4; x++)
            {
                for (int z = -Chunk.Chunk.ChunkSize * 4; z < Chunk.Chunk.ChunkSize * 4; z++)
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
