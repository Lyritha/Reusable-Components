using UnityEngine;

public class YawLook : Look
{

    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // rigidbody
        yaw += lookInput.x * yawSensitivity;
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(targetRot);
    }

    private void OnDestroy()
    {
        rb.angularVelocity = Vector3.zero;
    }
}
