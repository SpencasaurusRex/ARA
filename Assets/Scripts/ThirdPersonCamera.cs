using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    // Rotation
    public Vector2 MouseSensitivity = new Vector2(3, 3);
    public Vector2 PitchMinMax = new Vector2(-40, 85);
    public float RotationSmoothTime = .1f;
    public bool InvertVertical;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    float yaw;
    float pitch;

    // Zoom
    public float StartDistance = 4;
    public Vector2 DistanceMinMax = new Vector2(2, 20);
    public Vector2 ZoomSpeedMinMax = new Vector3(2, 10);
    public float ZoomSmoothTime = .5f;
    Line zoomSpeed;
    bool focusing;
    float currentDistance;
    float targetDistance;
    float currentZoomVelocity;
 
    // Movement
    public Vector2 Deadzone = new Vector2(.1f, .1f);
    public AnimationCurve movementSpeedFromZoom;
    public float MovementZoomRatio = .2f;
    public float MovementSmoothTime = .1f;
    public float SprintModifier = 1.3f;
    Vector3 currentMoveVelocity;
    Vector3 position;

    // Refocus
    public float SmoothRefocusMaxDistance = 80f;
    public float RefocusSmoothTimePerDistance = .01f;
    public float RefocusSmoothTimeBase = .05f;
    float refocusSmoothTime;
    Vector3 currentRefocusVelocity;
    bool refocusing;
    const float REFOCUS_EPSILON = .1f;
    float lastDistance;

    // A reference to the transform of the object we're targeting
    Transform objectTarget;
    // Our extra target, that will follow the object target when we're focused
    [SerializeField]
    Transform target;

    public void Focus(Transform newTarget)
    {
        refocusing = false;
        lastDistance = float.MaxValue;
        float distance;
        if (objectTarget == null)
        {
            distance = Vector3.Distance(target.position, newTarget.position);
        }
        else
        {
            distance = Vector3.Distance(objectTarget.position, newTarget.position);
        }
        if (distance <= SmoothRefocusMaxDistance)
        {
            refocusing = true;
            refocusSmoothTime = distance * RefocusSmoothTimePerDistance + RefocusSmoothTimeBase;
        }
        objectTarget = newTarget;
    }

    public void Unfocus()
    {
        position = target.position;
        objectTarget = null;
        refocusing = false;
    }

    private void Start()
    {
        currentDistance = targetDistance = StartDistance;
#if !UNITY_EDITOR
        zoomSpeed = new Line(DistanceMinMax.x, ZoomSpeedMinMax.x, DistanceMinMax.y, ZoomSpeedMinMax.y);
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        zoomSpeed = new Line(DistanceMinMax, ZoomSpeedMinMax);
#endif
        // Refocus
        if (refocusing)
        {
            // TODO
            refocusing = false;
            //target.position = Vector3.SmoothDamp(target.position, objectTarget.position, ref currentRefocusVelocity, refocusSmoothTime);
            //float distance = (target.position - objectTarget.position).sqrMagnitude;
            //Debug.Log((target.position - objectTarget.position).magnitude);
            //if (distance < REFOCUS_EPSILON)
            //{
            //    refocusing = false;
            //}
            //else if (lastDistance <= distance)
            //{
            //    // The camera is not catching up at this speed
            //    refocusSmoothTime *= .25f;
            //}
            //else
            //{
            //    lastDistance = distance;
            //    return;
            //}
        }
        
        // Movement
        Vector2 moveXY = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Mathf.Abs(moveXY.x) > Deadzone.x || Mathf.Abs(moveXY.y) > Deadzone.y)
        {
            if (objectTarget != null)
            {
                Unfocus();
            }
            Vector3 movement = transform.right * moveXY.x + transform.forward * moveXY.y;
            movement.y = 0;
            movement.Normalize();
            movement *= movementSpeedFromZoom.Evaluate(currentDistance) * Time.deltaTime;
            if (Input.GetButton("Sprint"))
            {
                movement *= SprintModifier;
            }
            position += movement;
        }
    }

    void LateUpdate() {
        if (objectTarget == null)
        {
            target.position = Vector3.SmoothDamp(target.position, position, ref currentMoveVelocity, MovementSmoothTime);
        }
        else if (!refocusing)
        {
            target.position = objectTarget.position;
        }

        // Rotation
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * MouseSensitivity.x;
            if (InvertVertical)
            {
                pitch -= Input.GetAxis("Mouse Y") * MouseSensitivity.y;
            }
            else
            {
                pitch += Input.GetAxis("Mouse Y") * MouseSensitivity.y;
            }
            pitch = Mathf.Clamp(pitch, PitchMinMax.x, PitchMinMax.y);
        }
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, RotationSmoothTime);
        transform.eulerAngles = currentRotation;
        
        targetDistance -= Input.mouseScrollDelta.y * zoomSpeed.EvaluateAt(targetDistance);
        targetDistance = Mathf.Clamp(targetDistance, DistanceMinMax.x, DistanceMinMax.y);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref currentZoomVelocity, ZoomSmoothTime);

        transform.position = target.position - transform.forward * currentDistance;
    }
}
