using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{
    public class TileObject : MonoBehaviour
    {
        #region Member variables
        // Unique ID
        public int id;

        // Stats
        public int movementTime = 50; // In ticks
        public int turnTime = 50; // In ticks

        // Temp states
        private short movementTicks;
        private short turningTicks;

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
            // TODO instantiation should set this
            var x = Random.Range(0, MovementManager.CHUNK_LENGTH);
            var y = 0;
            var z = Random.Range(0, MovementManager.CHUNK_LENGTH);
            position = new Vector3Int(x, y, z);
            transform.position = position;
            transform.rotation = Util.ToQuaternion(heading);
            MovementManager.RegisterTileObject(this);
        }

        void Update()
        {
            switch (action)
            {
                case MovementAction.Idle:
                    return;

                case MovementAction.GoForward:
                    // Check destination
                    break;

                case MovementAction.TurnLeft:
                    // Check rotation
                    break;
            }
        }
        #endregion

        public void Tick()
        {
            // If we're moving check to see if we're done
            Debug.Log("Tick on " + action);
            switch (action)
            {
                case MovementAction.GoForward:
                    movementTicks++;
                    transform.localPosition = Vector3.Lerp(position, targetPosition, (float)movementTicks / movementTime);
                    if (movementTicks == movementTime)
                    {
                        // We're done moving
                        MovementManager.Unblock(this.position);
                        position = targetPosition;
                        movementTicks = 0;
                        action = MovementAction.Idle;
                    }
                    break;
                case MovementAction.TurnLeft:
                case MovementAction.TurnRight:
                    turningTicks++;
                    transform.localRotation = Quaternion.Lerp(Util.ToQuaternion(heading), Util.ToQuaternion(targetHeading), (float)turningTicks / turnTime);
                    if (turningTicks == turnTime)
                    {
                        // We're done turning
                        heading = targetHeading;
                        turningTicks = 0;
                        action = MovementAction.Idle;
                    }
                    break;
            }
            // TODO add other actions

            // This is separate to ensure that movement will smoothly continue on the next tick if necessary
            if (action == MovementAction.Idle)
            {
                // TODO: replace placeholder code
                targetAction = (MovementAction)Random.Range(3, 6);
                //targetAction = MovementAction.GoForward;

                MovementManager.RegisterAction(this);
            }
        }
    }
}