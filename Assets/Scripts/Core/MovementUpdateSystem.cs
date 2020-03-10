using System;

namespace Assets.Scripts.Core
{
    public class MovementUpdateSystem : IUpdateSystem
    {
        public void Update(float fractional)
        {
            Console.WriteLine("MovementUpdateSystem: " + fractional);
        }
    }
}
