using UnityEngine;

public class TableLook : MonoBehaviour, ICharacterLook
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

    private float pitch;
    private float yaw;
    private Vector2 lookInput;
    private bool lookLock = true;

    private void Awake()
    {
        camController = GetComponentInChildren<CameraController>();

        if (camController != null)
        {
            camController.SetCamOffset(new(0, 0.65f, -0.8f), true);
            camController.SetCamTilt(0f, 35f);
        }
    }

    public void OnLook(Vector2 dir) => lookInput = dir;

    private void FixedUpdate()
    {
        if (camController == null || lookLock) return;

        yaw += lookInput.x * yawSensitivity;
        pitch -= lookInput.y * pitchSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        camController.SetRotation(yaw, pitch);
    }

    public void OnWantToLook(bool enableLook) => lookLock = !enableLook;
}
