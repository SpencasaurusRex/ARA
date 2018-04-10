using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class TileEntity : MonoBehaviour
    {
        public float DoubleClickDelay = .5f;
        public ulong Id;
        public int StartHeading;
        public int TicksPerTile = 50;
        public int TicksPerTurn = 50;
        public ulong scriptId;

        private float lastClick;

        private void Start()
        {
            
        }

        private void OnMouseDown()
        {
            if (Time.time - lastClick <= DoubleClickDelay)
            {
                // Double click
                Camera.main.GetComponent<ThirdPersonCamera>().Focus(transform);
            }
            else
            {
                // Single click
                lastClick = Time.time;
            }
        }
    }
}