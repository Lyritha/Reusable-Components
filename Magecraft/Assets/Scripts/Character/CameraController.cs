using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Camera cam;

    private Transform camT;

    // Smoothing
    private Vector3 targetOffset;
    private Vector3 currentOffset;
    [SerializeField] private float offsetSmoothSpeed = 10f;

    private void Awake()
    {
        camT = cam.transform;

        currentOffset = camT.localPosition;
        targetOffset = currentOffset;
    }

    public void SetCamOffset(Vector3 camOffset) => targetOffset = camOffset;
    public void SetRotation(float yaw, float pitch) => pivot.localRotation = Quaternion.Euler(pitch, yaw, 0f);


    private void LateUpdate()
    {
        // Smoothly interpolate camera offset
        currentOffset = Vector3.Lerp(
            currentOffset,
            targetOffset,
            offsetSmoothSpeed * Time.deltaTime
        );

        camT.localPosition = currentOffset;
    }
}
