﻿using System;

namespace Assets.Scripts.Core
{
    public class ScriptUpdateSystem : IUpdateSystem
    {
        public void Update(float fractional)
        {
            Console.WriteLine("ScriptUpdateSystem: " + fractional);
        }
    }
}
