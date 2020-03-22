using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

namespace ARACore
{
    public class UpdateManager
    {
        public const float TickLength = 0.3f;
        
        readonly List<IUpdateSystem> updateSystems = new List<IUpdateSystem>();
        float progress;
        float lastProgress;
        bool firstRun = true;

        public UpdateManager(params IUpdateSystem[] system)
        {
            updateSystems.AddRange(system);
        }

        public float Update(float deltaTime)
        {
            lastProgress = progress;
            progress += deltaTime / TickLength;

            if (firstRun)
            {
                AdvanceTick();
                firstRun = false;
            }

            UpdateSystems();
            // Handle when we cross a tick boundary
            while (progress >= 1)
            {
                AdvanceTick();
                lastProgress = 0;
                progress--;
                UpdateSystems();
            }

            return progress;
        }

        void AdvanceTick()
        {
            foreach (var system in updateSystems)
            {
                system.EndTick();
            }
        }

        void UpdateSystems()
        {
            for (int i = 0; i < updateSystems.Count; i++)
            {
                float systemProgress = GetSystemProgress(i, progress);
                if (systemProgress < 0) return;
                systemProgress = Mathf.Clamp01(systemProgress);

                float systemLastProgress = GetSystemProgress(i, lastProgress);

                if (systemLastProgress < 1)
                {
                    updateSystems[i].Update(systemProgress);
                }
            }
        }

        float GetSystemProgress(int index, float globalProgress) => updateSystems.Count * globalProgress - index;
    }
}