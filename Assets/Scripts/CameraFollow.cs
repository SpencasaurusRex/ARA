using UnityEngine;

namespace ARACore
{
    public class CameraFollow : MonoBehaviour
    {
        Transform movementTarget;
        Vector3 offset;

        public void Init(Transform target)
        {
            movementTarget = target;
            offset = Camera.main.transform.position;
        }

	    void Update ()
        {
            if (movementTarget != null)
                transform.position = movementTarget.position + offset;
	    }
    }
}
