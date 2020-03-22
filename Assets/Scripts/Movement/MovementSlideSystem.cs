using ARACore;
using Assets.Scripts.Chunk;
using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class MovementSlideSystem
    {
        EntitySet movementSet;
        EntitySet gridSet;

        public MovementSlideSystem(World world)
        {
            movementSet = world.GetEntities().With<Movement>().With<Translation>().AsSet();
            gridSet = world.GetEntities().With<Translation>().With<GridPosition>().Without<Movement>().AsSet();
        }

        public void Update(float fractional)
        {
            foreach (var entity in movementSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var movement = entity.Get<Movement>();

                translation.Value = movement.Start + movement.Direction.ToVector3() * fractional;
            }

            foreach (var entity in gridSet.GetEntities())
            {
                var translation = entity.Get<Translation>();
                var gridPosition = entity.Get<GridPosition>();

                translation.Value = gridPosition.Value.ToVector3();
            }
        }
    }
}
