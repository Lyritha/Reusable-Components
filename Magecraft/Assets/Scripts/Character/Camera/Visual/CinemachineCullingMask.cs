using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCameraEvents))]
public class CinemachineCullingMask : MonoBehaviour
{
    [SerializeField]
    private LayerMask targetCullingMask;

    private CinemachineCameraEvents cameraEvents;
    private CinemachineBrain brain;

    void Start()
    {
        cameraEvents = GetComponent<CinemachineCameraEvents>();
        brain = FindFirstObjectByType<CinemachineBrain>();

        cameraEvents.CameraActivatedEvent.AddListener(OnCameraActivated);
    }

    void OnCameraActivated(ICinemachineMixer mixer, ICinemachineCamera cam)
    {
        Debug.Log($"Camera Activated: {gameObject}");
        brain.OutputCamera.cullingMask = targetCullingMask;
    }
}
