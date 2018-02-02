using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class TileObject : MonoBehaviour
    {
        public enum Heading
        {
            East,
            North,
            West,
            South
        }

        #region Member variables
        public MovementManager movementManager;
        // Stats
        public int movementTime = 500; // 5 seconds

        // Temp states
        private short movementTicks;

        // Current states
        public Vector3Int position   = Vector3Int.zero;
        public Heading heading       = Heading.East;
        public MovementAction action = MovementAction.Idle;

        // Target states
        public Vector3Int targetPosition  = Vector3Int.zero;
        public Heading targetHeading      = Heading.East;
        public MovementAction targetAction = MovementAction.Idle;
        #endregion

        #region Unity Methods
        void Start()
        {
            // MUST have movementManager attached by this point
            if (movementManager == null)
            {
                Debug.LogError("Movement Manager not attached");
            }
            else
            {
                movementManager.RegisterTileObject(this);
            }
        }

        void Update()
        {
            switch (action)
            {
                case MovementAction.Idle:
                    return;

                case MovementAction.Forward:
                    // Check destination
                    break;

                case MovementAction.Left:
                    // Check rotation
                    break;
            }
        }
        #endregion

        public void Tick()
        {
            // If we're moving check to see if we're done
            if (action == MovementAction.Forward)
            {
                movementTicks++;
                transform.localPosition = Vector3.Lerp(position, targetPosition, (float)movementTicks / movementTime);
                if (movementTicks == movementTime)
                {
                    // We're done moving
                    // TODO testing bug detectiong
                    movementManager.Unblock(targetPosition);
                    //movementManager.Unblock(position);
                    position = targetPosition;
                    movementTicks = 0;
                    action = MovementAction.Idle;
                }
            }

            // TODO add other actions

            if (action == MovementAction.Idle)
            {
                // TODO: replace placeholder code
                //targetAction = (MovementAction)Random.Range(0, 7);
                targetAction = MovementAction.Forward;
                Debug.Log("Movement action = " + targetAction);

                movementManager.RegisterAction(this);
            }
        }
    }
}