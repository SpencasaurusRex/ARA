using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    public class RenderingSystem
    {
        World world;
        EntitySet meshSet;

        public RenderingSystem(World world)
        {
            this.world = world;
            meshSet = world.GetEntities().With<Mesh>().With<Material>().AsSet();
        }

        public void Update()
        {
            foreach (var meshEntity in meshSet.GetEntities())
            {
                var mesh = meshEntity.Get<Mesh>();
                var material = meshEntity.Get<Material>();
                Graphics.DrawMesh(mesh, Matrix4x4.identity, material, 0, Camera.main);
            }
        }
    }
}
