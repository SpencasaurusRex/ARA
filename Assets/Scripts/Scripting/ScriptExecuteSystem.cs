using System.Runtime.InteropServices;
using ARACore;
using Assets.Scripts.Chunk;
using Assets.Scripts.Movement;
using DefaultEcs;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.Analytics;
using Coroutine = MoonSharp.Interpreter.Coroutine;

namespace Assets.Scripts.Scripting
{
    public class ScriptExecuteSystem
    {
        EntitySet newScripts;
        EntitySet existingScripts;

        public ScriptExecuteSystem(World world)
        {
            newScripts = world.GetEntities().With<ScriptInfo>().Without<Coroutine>().AsSet();
            existingScripts = world.GetEntities().With<Coroutine>().With<ScriptStatus>().AsSet();
        }

        public void Update()
        {
            foreach (var entity in newScripts.GetEntities())
            {
                var info = entity.Get<ScriptInfo>();

                //string path = Path.Combine(Application.persistentDataPath, "Resources", "MoonSharp", info.Path);

                var script = new Script();
                script.DoFile(info.Path);

                //script.Globals["id"] = robotId;
                //script.Globals["__result"] = (Func<ulong, bool>) Result;
                //script.Globals["__x"] = (Func<ulong, long>)X;
                //script.Globals["__y"] = (Func<ulong, long>)Y;
                //script.Globals["__z"] = (Func<ulong, long>)Z;
                //script.Globals["__heading"] = (Func<ulong, int>)H;

                DynValue runFunction = script.Globals.Get("run");

                if (runFunction == DynValue.Nil)
                {
                    info.Status = ScriptStatus.Done;
                    continue;
                }

                var coroutine = script.CreateCoroutine(runFunction).Coroutine;
                entity.Set(coroutine);
                info.Status = ScriptStatus.Running;
            }

            foreach (var entity in existingScripts.GetEntities())
            {
                var coroutine = entity.Get<Coroutine>();

                if (coroutine.State == CoroutineState.Suspended
                    || coroutine.State == CoroutineState.NotStarted
                    || coroutine.State == CoroutineState.ForceSuspended)
                {
                    DynValue action = coroutine.Resume();
                    if (action.Type == DataType.Number)
                    {
                        entity.Set((ScriptCommand)action.Number);
                    }
                }
            }
        }
    }
}
