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
                new MovementUpdateSystem(world)
            );

            renderingSystem = new RenderingSystem(world);
            transformWriteSystem = new TransformWriteSystem(world);
            movementSlideSystem = new MovementSlideSystem(world);

            Setup();
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
            Robot(Vector3Int.zero, new Vector3Int(0, 0, 1));
            Robot(new Vector3Int(0, 0, 20), new Vector3Int(0, 0, -1));
        }

        Entity Robot(Vector3Int initialPosition, Vector3Int direction)
        {
            var entity = world.CreateEntity();
            entity.Set(mesh);
            entity.Set(material);
            entity.Set(new LocalToWorld());
            entity.Set(new Translation { Value = initialPosition });
            entity.Set(new Rotation());
            entity.Set(new GridPosition { Value = initialPosition});
            entity.Set(new DesiredMovement { Value = direction });

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
