using UnityEngine;

public class ThirdPersonOrbitalLook : MonoBehaviour, ICharacterLook
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

    private float yaw;
    private float pitch;
    private Vector2 lookInput;

    private void Awake()
    {
        camController = GetComponentInChildren<CameraController>();

        if (camController != null)
        {
            camController.SetCamOffset(new(0, 1, -3));
            camController.SetRotation(0f, 0f);
        }

        yaw = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnLook(Vector2 dir) => lookInput = dir;

    private void Update()
    {
        if (camController == null) return;

        yaw += lookInput.x * yawSensitivity;
        pitch -= lookInput.y * pitchSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        camController.SetRotation(yaw, pitch);
    }

    public void OnWantToLook(bool enableLook) { }
}
