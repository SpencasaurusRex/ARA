using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class TileEntity : MonoBehaviour
    {
        public ulong id;
        public int startHeading;
        public int ticksPerTile = 50;
        public int ticksPerTurn = 50;

        private void Start()
        {
            
        }
    }
}