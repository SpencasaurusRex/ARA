using System;

namespace Assets.Scripts.Core
{
    public class TileEntityUpdateSystem : IUpdateSystem
    {
        public void Update(float fractional)
        {
            Console.WriteLine("TileEntityUpdateSystem: " + fractional);
        }
    }
}