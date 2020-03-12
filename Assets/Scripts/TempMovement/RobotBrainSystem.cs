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
            translationSet = world.GetEntities().With<GridPosition>().AsSet();
            movementResultSet = world.GetEntities().With<MovementResult>().AsSet();
        }

        public void Update(float fractional)
        {
            if (1.0 - fractional > float.Epsilon) return;

            foreach (var entity in translationSet.GetEntities())
            {
                var currentPosition = entity.Get<GridPosition>().Value;

                int id = entity.Get<ID>().Value;
                var targetX = Mathf.FloorToInt(id / 6) + id % 6 + 12;
                var targetZ = Mathf.FloorToInt(id / 6) + 12;
                var targetY = 0;

                var calculatedDirection = Vector3Int.zero;
                if (currentPosition.x != targetX)
                {
                    calculatedDirection.x = (int)Mathf.Sign(targetX - currentPosition.x);
                }
                else if (currentPosition.z != targetZ)
                {
                    calculatedDirection.z = (int) Mathf.Sign(targetZ - currentPosition.z);
                }
                else if (currentPosition.y != targetY)
                {
                    calculatedDirection.y = (int) Mathf.Sign(targetY - currentPosition.y);
                }
                else continue;

                //Debug.Log($"Requesting movement from {currentPosition} to {currentPosition + calculatedDirection}");
                entity.Set(new MovementRequest(currentPosition, currentPosition + calculatedDirection));
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
