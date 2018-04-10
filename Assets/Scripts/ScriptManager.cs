using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    ulong currentId;
    Dictionary<ulong, MoonSharp.Interpreter.Coroutine> coroutines;

    private void Awake()
    {
        coroutines = new Dictionary<ulong, MoonSharp.Interpreter.Coroutine>();
    }

    public ulong CreateScript()
    {
        var id = currentId++;
        var script = new Script();
        script.DoFile("script");
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
