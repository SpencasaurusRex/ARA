using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform movementTarget;
    public Transform lookTarget;
    public float movementRigidity = 1f;
    public float lookRigidity = 1f;
	
	void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, movementTarget.position, movementRigidity * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTarget.position - transform.position), lookRigidity * Time.deltaTime);
	}
}
