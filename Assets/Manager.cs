using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARACore
{

    public class Manager : MonoBehaviour {
        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < MovementManager.CHUNK_LENGTH; i++)
            {
                for (int j = 0; j < MovementManager.CHUNK_HEIGHT; j++)
                {
                    for (int k = 0; k < MovementManager.CHUNK_LENGTH; k++)
                    {
                        var loc = new Vector3Int(i, j, k);
                        if (MovementManager.blocked.ContainsKey(loc))
                        {
                            Gizmos.color = new Color(1, 1, 0, 0.75F);
                            Gizmos.DrawCube(loc, Vector3.one * .99f);
                        }
                        else
                        {
                            Gizmos.color = new Color(.2f, .8f, .2f, 0.5f);
                            Gizmos.DrawCube(loc, Vector3.one * 0.1f);
                        }
                    }
                }
            }
        }

        void FixedUpdate()
        {
            MovementManager.Tick();
        }
    }
}