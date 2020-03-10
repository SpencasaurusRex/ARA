using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

namespace ARACore
{
    public class UpdateManager : MonoBehaviour
    {
        // Configuration
        public float TickLength = 0.5f;
        
        // Runtime
        List<IUpdateSystem> updateSystems = new List<IUpdateSystem>();
        float progress = -0.01f;
        float lastProgress;

        void Awake()
        {
            updateSystems.Add(new TileEntityUpdateSystem());
            updateSystems.Add(new ScriptUpdateSystem());
            updateSystems.Add(new MovementUpdateSystem());
        }

        void Update()
        {
            lastProgress = progress;
            progress += Time.deltaTime / TickLength;

            UpdateSystems();
            // Handle when we cross a tick boundary
            while (progress > 1)
            {
                lastProgress = 0;
                progress--;
                UpdateSystems();
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