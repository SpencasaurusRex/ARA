using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Transform
{
    public class TransformWriteSystem
    {
        World world;
        EntitySet translateSet;
        EntitySet translateRotateSet;

        public TransformWriteSystem(World world)
        {
            this.world = world;
            translateSet = world.GetEntities().With<Translation>().Without<Rotation>().With<LocalToWorld>().AsSet();
            translateRotateSet = world.GetEntities().With<Translation>().With<Rotation>().With<LocalToWorld>().AsSet();
        }

        public void Update()
        {
            foreach (var entity in translateSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var localToWorld = entity.Get<LocalToWorld>();

                localToWorld.Matrix = Matrix4x4.Translate(translation.Value);
            }

            foreach (var entity in translateRotateSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var rotation = entity.Get<Rotation>();
                var localToWorld = entity.Get<LocalToWorld>();

                localToWorld.Matrix = Matrix4x4.TRS(translation.Value, rotation.Value, Vector3.one);
            }
        }
    }
}
