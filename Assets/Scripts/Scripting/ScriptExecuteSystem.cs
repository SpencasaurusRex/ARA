using Assets.Scripts.Core;
using Assets.Scripts.Movement;
using DefaultEcs;
using MoonSharp.Interpreter;
using UnityEngine;
using Coroutine = MoonSharp.Interpreter.Coroutine;

namespace Assets.Scripts.Scripting
{
    public class ScriptExecuteSystem : IUpdateSystem
    {
        EntitySet newScripts;
        EntitySet existingScripts;
        EntitySet actionResultSet;

        public ScriptExecuteSystem(World world)
        {
            newScripts = world.GetEntities().With<ScriptInfo>().Without<Coroutine>().AsSet();
            existingScripts = world.GetEntities().With<Coroutine>().AsSet();
            actionResultSet = world.GetEntities().With<ActionResult>().AsSet();
        }

        public void Update(float fractional)
        {
            if (fractional != 1.0f) return;

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

                entity.Set(script);
                entity.Set(script.CreateCoroutine(runFunction).Coroutine);
                info.Status = ScriptStatus.Running;
            }

            foreach (var entity in existingScripts.GetEntities())
            {
                var coroutine = entity.Get<Coroutine>();
                var script = entity.Get<Script>();

                if (entity.Has<ActionResult>())
                {
                    var result = entity.Get<ActionResult>().Result;
                    script.Globals["__result"] = result;
                }

                // Note: coroutine state is Dead when done

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

            
            foreach (var entity in actionResultSet.GetEntities())
            {
                entity.Remove<ActionResult>();
            }
        }

        public void EndTick()
        {
            
        }
    }
}
