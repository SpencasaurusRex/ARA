using System;
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

    // A reference to the transform of the object we're targeting
    Transform focus;
    public Transform CameraBase;

    public void Focus(Transform newFocus)
    {
        focus = newFocus;
    }

    public void Unfocus()
    {
        focus = null;
    }

    void Start()
    {
        position = CameraBase.position;
        currentDistance = targetDistance = StartDistance;
    }

    void Update()
    {
        // Movement
        Vector2 moveXY = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Mathf.Abs(moveXY.x) > Deadzone.x || Mathf.Abs(moveXY.y) > Deadzone.y)
        {
            if (focus != null)
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

        float movementProgress = 1f - Mathf.Exp(-5 * Time.deltaTime);
        CameraBase.position = Vector3.Lerp(CameraBase.position, position, movementProgress);
    }

    void LateUpdate()
    {

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
        
        targetDistance -= Input.mouseScrollDelta.y;
        targetDistance = Mathf.Clamp(targetDistance, DistanceMinMax.x, DistanceMinMax.y);
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref currentZoomVelocity, ZoomSmoothTime);

        transform.position = CameraBase.position - transform.forward * currentDistance;
    }
}
