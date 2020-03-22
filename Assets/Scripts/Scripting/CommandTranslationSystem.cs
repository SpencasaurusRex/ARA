using System.Linq;
using Assets.Scripts.Chunk;
using Assets.Scripts.Core;
using Assets.Scripts.Movement;
using DefaultEcs;
using UnityEngine;

namespace Assets.Scripts.Scripting
{
    public class CommandTranslationSystem : IUpdateSystem
    {
        EntitySet scriptCommandSet;

        public CommandTranslationSystem(World world)
        {
            scriptCommandSet = world.GetEntities().With<ScriptCommand>().AsSet();
        }

        ScriptCommand[] movementCommands =
        {
            ScriptCommand.Forward,
            ScriptCommand.Back,
            ScriptCommand.Up,
            ScriptCommand.Down,
        };

        ScriptCommand[] turnCommands =
        {
            ScriptCommand.Left,
            ScriptCommand.Right
        };

        public void Update(float fractional)
        {
            if (fractional != 1.0f) return;

            foreach (var entity in scriptCommandSet.GetEntities())
            {
                var command = entity.Get<ScriptCommand>();

                if (movementCommands.Contains(command))
                {
                    Movement(entity, command);
                }
                else if (turnCommands.Contains(command))
                {
                    Turn(entity, command);
                }

                entity.Remove<ScriptCommand>();
            }
        }

        static Vector3Int[] CardinalDirections =
        {
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0),
        };

        public void Movement(Entity entity, ScriptCommand command)
        {
            var heading = (int) entity.Get<CardinalHeading>();
            var position = entity.Get<GridPosition>().Value;

            Vector3Int direction = Vector3Int.zero;
            switch (command)
            {
                case ScriptCommand.Forward:
                    direction = CardinalDirections[heading];
                    break;

                case ScriptCommand.Back:
                    direction = -CardinalDirections[heading];
                    break;

                case ScriptCommand.Up:
                    direction = Vector3Int.up;
                    break;

                case ScriptCommand.Down:
                    direction = Vector3Int.down;
                    break;
            }

            entity.Set(new MovementRequest(position, position + direction));
        }

        void Turn(Entity entity, ScriptCommand command)
        {
            var heading = entity.Get<CardinalHeading>();

            CardinalHeading targetHeading;

            if (command == ScriptCommand.Right)
            {
                targetHeading = heading.Clockwise();
            }
            else
            {
                targetHeading = heading.CounterClockwise();
            }

            entity.Set(new TurnRequest(heading, targetHeading));
        }

        public void EndTick()
        {
        }
    }
}
