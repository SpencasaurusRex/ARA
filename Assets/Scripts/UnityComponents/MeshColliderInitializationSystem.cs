using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.UnityComponents
{
    public class MeshColliderInitializationSystem : UnityInitializer
    {
        World world;
        EntitySet meshColldierInitializers;

        public MeshColliderInitializationSystem(World world) : base(world)
        {
            this.world = world;
            meshColldierInitializers = world.GetEntities().With<MeshColliderInitializer>().AsSet();
            SetOperationSets(meshColldierInitializers);
        }

        public override void Update()
        {
            foreach (var entity in meshColldierInitializers.GetEntities())
            {
                var initializer = entity.Get<MeshColliderInitializer>();
                GameObject go = GetGameObject(entity);

                MeshCollider meshCollider = go.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = initializer.Mesh;

                go.transform.position = initializer.Position;

                entity.Remove<MeshColliderInitializer>();
            }
        }
    }
}
