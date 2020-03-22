﻿using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using Assets.Scripts.Scripting;
using Assets.Scripts.TempMovement;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class RobotBrainSystem : IUpdateSystem
    {
        EntitySet translationSet;

        public RobotBrainSystem(World world)
        {
            translationSet = world.GetEntities().With<GridPosition>().AsSet();
        }

        int tickNumber;
        public void Update(float fractional)
        {
            if (fractional != 1.0f) return;

            tickNumber++;
            foreach (var entity in translationSet.GetEntities())
            {
                //ZigZag(entity);
                //Turn(entity);
                Dance(entity);
            }
        }

        void Dance(Entity entity)
        {
            if (tickNumber % 2 == 0)
            {
                entity.Set(ScriptCommand.Forward);
            }
            else
            {
                entity.Set(ScriptCommand.Left);
            }
            
        }

        void Turn(Entity entity)
        {
            entity.Set(ScriptCommand.Left);
        }

        void ZigZag(Entity entity)
        {
            var currentPosition = entity.Get<GridPosition>().Value;
            int lineWidth = 16;
            int id = entity.Get<ID>().Value;
            var targetX = Mathf.FloorToInt(id / lineWidth) + id % lineWidth + 16;
            var targetZ = Mathf.FloorToInt(id / lineWidth) + 16;
            var targetY = 0;

            var calculatedDirection = Vector3Int.zero;
            var r = Random.Range(0f, 1f);

            if (entity.Has<ActionResult>() && entity.Get<ActionResult>().Result == false && r < 0.5f)
            {
                calculatedDirection.y = 1;
            }
            else if (currentPosition.x != targetX /*&& r < 0.7f*/)
            {
                calculatedDirection.x = (int)Mathf.Sign(targetX - currentPosition.x);
            }
            else if (currentPosition.z != targetZ/* && r < 0.9f*/)
            {
                calculatedDirection.z = (int) Mathf.Sign(targetZ - currentPosition.z);
            }
            else if (currentPosition.y != targetY)
            {
                calculatedDirection.y = (int) Mathf.Sign(targetY - currentPosition.y);
            }
            else return;

            entity.Set(new MovementRequest(currentPosition, currentPosition + calculatedDirection));
        }

        public void EndTick()
        {
            
        }
    }
}
