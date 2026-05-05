using UnityEngine;

public class YawLook : MonoBehaviour, ICharacterLook
{
    [SerializeField]
    private float yawSensitivity = 0.5f;

    private Rigidbody rb;

    private float yaw;
    private Vector2 lookInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        yaw = transform.eulerAngles.y;
    }

    public void OnLook(Vector2 dir) => lookInput = dir;

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
