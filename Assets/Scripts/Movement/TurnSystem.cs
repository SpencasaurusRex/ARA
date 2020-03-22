using Assets.Scripts.Chunk;
using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class TurnSystem
    {
        EntitySet turnSet;
        EntitySet headingSet;

        public TurnSystem(World world)
        {
            turnSet = world.GetEntities().With<Turn>().AsSet();
            headingSet = world.GetEntities().With<CardinalHeading>().Without<Turn>().AsSet();
        }

        public void Update(float fractional)
        {
            foreach (var entity in turnSet.GetEntities())
            {
                var turn = entity.Get<Turn>();
                var rotation = entity.Get<Rotation>();

                rotation.Value = Quaternion.Slerp(turn.FromQuaternion, turn.ToQuaternion, fractional);
            }

            foreach (var entity in headingSet.GetEntities())
            {
                var heading = entity.Get<CardinalHeading>();
                var rotation = entity.Get<Rotation>();

                rotation.Value = heading.ToQuaternion();
            }
        }
    }
}
