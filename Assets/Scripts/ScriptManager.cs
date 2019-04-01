using MoonSharp.Interpreter;
using System.Collections.Generic;
using System;

namespace ARACore
{
    public class ScriptManager
    {
        ulong currentId;
        Dictionary<ulong, MoonSharp.Interpreter.Coroutine> coroutines;
        
        public ScriptManager()
        {
            coroutines = new Dictionary<ulong, MoonSharp.Interpreter.Coroutine>();
        }

        private static bool Result(ulong id)
        {
            MovementResultType type = Manager.movement.movementResult[id].type;
            switch (type)
            {
                //case MovementResultType.AlreadyMoving:
                case MovementResultType.Blocked:
                case MovementResultType.OutOfFuel:
                    return false;
                case MovementResultType.DoneMoving:
                    return true;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static long X(ulong id)
        {
            return Manager.movement.movementEntities[id].tilePosition.x;
        }

        private static long Y(ulong id)
        {
            return Manager.movement.movementEntities[id].tilePosition.y;
        }

        private static long Z(ulong id)
        {
            return Manager.movement.movementEntities[id].tilePosition.z;
        }

        private static int H(ulong id)
        {
            return Manager.movement.movementEntities[id].heading;
        }

        public ulong CreateScript(ulong robotId)
        {
            var id = currentId++;
            var script = new Script();
            //int index = Random.Range(0, 9);
            script.DoFile("script" + 0);
            script.Globals["id"] = robotId;
            script.Globals["__result"] = (Func<ulong, bool>) Result;
            script.Globals["__x"] = (Func<ulong, long>)X;
            script.Globals["__y"] = (Func<ulong, long>)Y;
            script.Globals["__z"] = (Func<ulong, long>)Z;
            script.Globals["__heading"] = (Func<ulong, int>)H;
            DynValue dynRun = script.CreateCoroutine(script.Globals.Get("run"));
            coroutines[id] = dynRun.Coroutine;
            return id;
        }

        public int Run(ulong coroutineId)
        {
            MoonSharp.Interpreter.Coroutine dynCoroutine = coroutines[coroutineId];
            if (dynCoroutine.State == CoroutineState.Suspended
                || dynCoroutine.State == CoroutineState.NotStarted
                || dynCoroutine.State == CoroutineState.ForceSuspended)
            {
                DynValue action = dynCoroutine.Resume();
                if (action.Type == DataType.Number)
                {
                    return (int)action.Number;
                }
            }
            return -1;
            // TODO check if the script is done running
        }
    }
}