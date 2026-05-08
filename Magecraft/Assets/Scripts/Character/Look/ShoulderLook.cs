using UnityEngine;

public class ShoulderLook : MonoBehaviour, ICharacterLook
{
    [SerializeField]
    private float yawSensitivity = 0.5f;
    [SerializeField]
    private float pitchSensitivity = 0.5f;

    [Header("Pitch Limits"), SerializeField]
    private float minPitch = -80f;
    [SerializeField]
    private float maxPitch = 80f;

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
            camController.SetCamOffset(new(0.7f, 0.15f, -1f));
            camController.SetRotation(0f, 0f);
        }

        yaw = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook(Vector2 dir) => lookInput = dir;

    private void FixedUpdate()
    {
        yaw += lookInput.x * yawSensitivity;
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(targetRot);

        if (camController == null) return;

        pitch -= lookInput.y * pitchSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        camController.SetRotation(0, pitch);
    }

    public void OnWantToLook(bool enableLook)
    {
    }
}
