using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using Assets.Scripts.TempMovement;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class RobotBrainSystem : IUpdateSystem
    {
        EntitySet translationSet;
        EntitySet movementResultSet;

        public RobotBrainSystem(World world)
        {
            translationSet = world.GetEntities().With<GridPosition>().With<DesiredMovement>().AsSet();
            movementResultSet = world.GetEntities().With<MovementResult>().AsSet();
        }

        public void Update(float fractional)
        {
            Debug.Log("RobotBrain: " + fractional);
            if (1.0 - fractional > float.Epsilon) return;

            foreach (var entity in translationSet.GetEntities())
            {
                var currentPosition = entity.Get<GridPosition>().Value;
                var desiredDirection = entity.Get<DesiredMovement>().Value;

                entity.Set(new MovementRequest(currentPosition, currentPosition + desiredDirection));
            }

            // TODO: This will need to be moved to the script manager
            foreach (var entity in movementResultSet.GetEntities())
            {
                entity.Remove<MovementResult>();
            }
        }

        public void EndTick()
        {
            
        }
    }
}
