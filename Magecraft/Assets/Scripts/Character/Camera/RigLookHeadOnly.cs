using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigLookHeadOnly : Look
{
    [SerializeField] private MultiAimConstraint constraint;

    [Header("Body Follow Settings")]
    [SerializeField] private float bodyTurnThreshold = 30f;   // degrees
    [SerializeField] private float bodyTurnSpeed = 5f;         // smoothing

    private float minPitch;
    private float maxPitch;

    private Transform head;
    private Transform target;

    protected override void Awake()
    {
        base.Awake();

        constraint = GetComponentInChildren<MultiAimConstraint>();

        head = constraint.data.constrainedObject;
        target = constraint.data.sourceObjects[0].transform;

        constraint.data.constrainedXAxis = false;
        constraint.data.constrainedYAxis = false;
        constraint.data.constrainedZAxis = false;

        minPitch = -360;
        maxPitch = 360;

        constraint.data.limits = new(minPitch, maxPitch);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        // Update head yaw from mouse
        yaw += lookInput.x * yawSensitivity;
    }

    protected override void Update()
    {
        base.Update();

        // Pitch rotation
        pitch -= lookInput.y * pitchSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (head != null && target != null)
        {
            // Head direction based on full yaw (not bodyYaw)
            Vector3 dir = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;

            // Move target in front of head
            target.position = head.position + dir * 1.0f;
        }
    }
}
