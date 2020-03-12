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
        World world;
        UpdateManager updateManager;
        RenderingSystem renderingSystem;
        TransformWriteSystem transformWriteSystem;
        MovementSlideSystem movementSlideSystem;
        SetBlockSystem setBlockSystem;

        public Material material;
        public Mesh mesh;

        void Start()
        {
            world = new World();

            updateManager = new UpdateManager
            (
                new TileEntityUpdateSystem(),
                new RobotBrainSystem(world),
                new ScriptUpdateSystem(),
                new MovementUpdateSystem(world),
                setBlockSystem = new SetBlockSystem(world)
            );

            renderingSystem = new RenderingSystem(world);
            transformWriteSystem = new TransformWriteSystem(world);
            movementSlideSystem = new MovementSlideSystem(world);

            Setup();

            setBlockSystem.EndTick();
        }

        void Update()
        {
            float fractional = updateManager.Update(Time.deltaTime);
            movementSlideSystem.Update(fractional);
            transformWriteSystem.Update();
            renderingSystem.Update();
        }

        void Setup()
        {
            var global = world.CreateEntity();
            global.Set(new Global());
            global.Set(new ChunkSet());

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    for (int z = 0; z < 15; z++)
                    {
                        Robot(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        static int id;
        Entity Robot(Vector3Int initialPosition)
        {
            var entity = world.CreateEntity();
            entity.Set(mesh);
            entity.Set(material);
            entity.Set(new LocalToWorld());
            entity.Set(new Translation { Value = initialPosition });
            entity.Set(new Rotation());
            entity.Set(new GridPosition { Value = initialPosition});
            entity.Set(new SetBlock {Block = Block.Robot});
            entity.Set(new Scale { Value = new Vector3(.9f, .9f, .9f)});
            entity.Set(new ID {Value = id++});

            return entity;
        }

        void OnDrawGizmos()
        {
            if (renderingSystem?.GizmoRenderList == null) return;
            foreach (var gizmoRender in renderingSystem.GizmoRenderList)
            {
                Gizmos.DrawMesh(gizmoRender.Mesh, gizmoRender.Translation, gizmoRender.Quaternion, gizmoRender.Scale);
            }
        }
    }
}
