using System.Net.Http.Headers;
using Assets.Scripts.Transform;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class TurnSystem
    {
        EntitySet turnSet;

        public TurnSystem(World world)
        {
            turnSet = world.GetEntities().With<Turn>().AsSet();
        }

        public void Update(float fractional)
        {
            foreach (var entity in turnSet.GetEntities())
            {
                var turn = entity.Get<Turn>();
                var rotation = entity.Get<Rotation>();

                rotation.Value = Quaternion.Slerp(turn.FromQuaternion, turn.ToQuaternion, fractional);
            }
        }
    }
}
