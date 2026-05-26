using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rigidbody))]
public class RigLook : Look
{
    [SerializeField] private MultiAimConstraint constraint;

    [Header("Body Follow Settings")]
    [SerializeField] private float bodyTurnThreshold = 30f;
    [SerializeField] private float bodyTurnSpeed = 6f;
    [SerializeField] private CinemachinePanTilt panTilt;

    private float minPitch;
    private float maxPitch;

    private Transform head;
    private Transform target;
    private Rigidbody rb;

    private float bodyYaw;
    private float bodyYawVel;
    private bool shouldTurn;

    private Vector3 smoothedDir = Vector3.forward;


    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // IMPORTANT when rotating in Update

        constraint = GetComponentInChildren<MultiAimConstraint>();
        panTilt = GetComponentInChildren<CinemachinePanTilt>();

        head = constraint.data.constrainedObject;
        target = constraint.data.sourceObjects[0].transform;

        constraint.data.constrainedXAxis = true;
        constraint.data.constrainedYAxis = true;
        constraint.data.constrainedZAxis = true;

        minPitch = -60f;
        maxPitch = 40f;
        constraint.data.limits = new(minPitch, maxPitch);

        yaw = bodyYaw = WrapAngle(transform.eulerAngles.y);

        target.SetParent(null, true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    protected override void Update()
    {
        base.Update();

        // --- CAMERA INPUT PIPELINE ---
        yaw += lookInput.x * yawSensitivity;
        yaw = WrapAngle(yaw);

        pitch -= lookInput.y * pitchSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (panTilt != null)
        {
            float camYaw = Mathf.DeltaAngle(bodyYaw, yaw);
            panTilt.PanAxis.Value = camYaw;
            panTilt.TiltAxis.Value = pitch;
        }

        // --- BODY ROTATION PIPELINE (NOW IN UPDATE) ---
        float headYawOffset = Mathf.DeltaAngle(bodyYaw, yaw);

        // hysteresis
        if (shouldTurn)
            shouldTurn = Mathf.Abs(headYawOffset) > 0.5f;
        else
            shouldTurn = Mathf.Abs(headYawOffset) > bodyTurnThreshold ||
                         rb.linearVelocity.magnitude > 0.1f;

        if (shouldTurn)
        {
            float offsetFactor = Mathf.Clamp01(headYawOffset / bodyTurnThreshold);
            float dynamicSmoothTime = Mathf.Lerp(
                1f / (bodyTurnSpeed * 1f),
                1f / (bodyTurnSpeed * 3f),
                offsetFactor
            );

            float maxSpeed = bodyTurnSpeed * 200f;

            bodyYaw = Mathf.SmoothDampAngle(
                bodyYaw,
                yaw,
                ref bodyYawVel,
                dynamicSmoothTime,
                maxSpeed,
                Time.deltaTime
            );
        }
        else
        {
            bodyYawVel = 0f;
        }

        // apply rotation directly
        transform.rotation = Quaternion.Euler(0f, bodyYaw, 0f);
    }


    private void LateUpdate()
    {
        if (panTilt != null)
        {
            Vector3 camForward = panTilt.transform.forward;
            float t = 1f - Mathf.Exp(-20f * Time.deltaTime);

            smoothedDir = Vector3.Slerp(smoothedDir, camForward, t);
            target.position = head.position + smoothedDir * 1f;
        }
    }


    private static float WrapAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }
}
