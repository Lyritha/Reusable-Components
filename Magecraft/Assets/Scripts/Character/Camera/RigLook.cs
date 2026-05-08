using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigLook : Look
{
    [SerializeField] private MultiAimConstraint constraint;

    [Header("Body Follow Settings")]
    [SerializeField] private float bodyTurnThreshold = 30f;   // degrees
    [SerializeField] private float bodyTurnSpeed = 5f;         // smoothing

    private float minPitch;
    private float maxPitch;

    private Transform head;
    private Transform target;
    private Rigidbody rb;

    private float bodyYaw;   // actual body rotation


    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        constraint = GetComponentInChildren<MultiAimConstraint>();

        head = constraint.data.constrainedObject;
        target = constraint.data.sourceObjects[0].transform;

        constraint.data.constrainedXAxis = true;
        constraint.data.constrainedYAxis = true;
        constraint.data.constrainedZAxis = true;

        minPitch = -60;
        maxPitch = 40;

        constraint.data.limits = new(minPitch, maxPitch);

        bodyYaw = transform.eulerAngles.y;
        yaw = bodyYaw;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        // Update head yaw from mouse
        yaw += lookInput.x * yawSensitivity;

        // Compute yaw offset between head and body
        float headYawOffset = Mathf.DeltaAngle(bodyYaw, yaw);

        // If head turns too far, rotate body to catch up
        if (Mathf.Abs(headYawOffset) > bodyTurnThreshold || rb.linearVelocity.magnitude > 0.01f)
            bodyYaw += headYawOffset * bodyTurnSpeed * Time.fixedDeltaTime;

        // Apply body rotation
        rb.MoveRotation(Quaternion.Euler(0f, bodyYaw, 0f));
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
