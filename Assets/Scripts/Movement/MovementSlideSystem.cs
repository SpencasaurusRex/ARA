using ARACore;
using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class MovementSlideSystem
    {
        EntitySet movementSet;

        public MovementSlideSystem(World world)
        {
            movementSet = world.GetEntities().With<Movement>().With<Translation>().AsSet();
        }

        public void Update(float fractional)
        {
            Debug.Log("MovementSlide: " + fractional);
            foreach (var entity in movementSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var movement = entity.Get<Movement>();

                translation.Value = movement.Start + movement.Direction.ToVector3() * fractional;
            }
        }
    }
}
