using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonLook : MonoBehaviour, ICharacterLook
{
    [SerializeField]
    private float yawSensitivity = 0.5f;
    [SerializeField]
    private float pitchSensitivity = 0.5f;

    [Header("Pitch Limits"), SerializeField]
    private float minPitch = -60f;
    [SerializeField]
    private float maxPitch = 60f;
    
    private CameraController camController;
    private Rigidbody rb;

    private float yaw;
    private float pitch;
    private Vector2 lookInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camController = GetComponentInChildren<CameraController>();

        if (camController != null)
        {
            camController.SetCamOffset(Vector3.zero);
            camController.SetRotation(0f, 0f);
        }

        yaw = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook(Vector2 dir) => lookInput = dir;

    private void Update()
    {
        yaw += lookInput.x * yawSensitivity;

        if (camController != null)
        {
            pitch -= lookInput.y * pitchSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            camController.SetRotation(0, pitch);
        }
    }

    private void FixedUpdate()
    {
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(targetRot);
    }

    private void OnDestroy()
    {
        rb.angularVelocity = Vector3.zero;
    }
}
