using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace ARACore
{
    public class ScriptManager
    {
        //Dictionary<ulong, Coroutine> coroutines;
        
        public ScriptManager()
        {
        }

        static bool Result(ulong id)
        {
            return true;
        }

        static long X(ulong id)
        {
            return 0;
        }

        static long Y(ulong id)
        {
            return 0;
        }

        static long Z(ulong id)
        {
            return 0;
        }

        static int H(ulong id)
        {
            return 0;
        }

        public void CreateScript(ulong robotId)
        {
            //var script = new Script();
            //script.DoFile("script" + 0);
            //script.Globals["id"] = robotId;
            //script.Globals["__result"] = (Func<ulong, bool>) Result;
            //script.Globals["__x"] = (Func<ulong, long>)X;
            //script.Globals["__y"] = (Func<ulong, long>)Y;
            //script.Globals["__z"] = (Func<ulong, long>)Z;
            //script.Globals["__heading"] = (Func<ulong, int>)H;
            //DynValue dynRun = script.CreateCoroutine(script.Globals.Get("run"));
            //coroutines[id] = dynRun.Coroutine;
            //return id;
        }

        public int Run(ulong coroutineId)
        {
            //Coroutine dynCoroutine = coroutines[coroutineId];
            //if (dynCoroutine.State == CoroutineState.Suspended
            //    || dynCoroutine.State == CoroutineState.NotStarted
            //    || dynCoroutine.State == CoroutineState.ForceSuspended)
            //{
            //    DynValue action = dynCoroutine.Resume();
            //    if (action.Type == DataType.Number)
            //    {
            //        return (int)action.Number;
            //    }
            return -1;
        }
    }
}