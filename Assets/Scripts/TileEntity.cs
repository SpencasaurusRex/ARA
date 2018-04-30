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

        float lastClick;

        void OnMouseDown()
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

        private void OnDestroy()
        {
            Manager.robotManager.Unassign(Id);
            Manager.movement.DestroyTileEntity(this);
        }
    }
}