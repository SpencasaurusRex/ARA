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
        List<IUpdateSystem> UpdateSystems = new List<IUpdateSystem>();
        float progress;

        void Awake()
        {
            UpdateSystems.Add(new TileEntityUpdateSystem());
            UpdateSystems.Add(new ScriptUpdateSystem());
            UpdateSystems.Add(new MovementUpdateSystem());
        }

        void Update()
        {
            
        }
    }
}