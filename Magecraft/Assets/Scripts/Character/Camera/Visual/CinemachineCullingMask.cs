using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCameraEvents))]
public class CinemachineCullingMask : MonoBehaviour
{
    [SerializeField]
    private LayerMask targetCullingMask;

    private CinemachineCameraEvents cameraEvents;
    private CinemachineBrain brain;

    private void Start()
    {
        cameraEvents = GetComponent<CinemachineCameraEvents>();
        brain = FindFirstObjectByType<CinemachineBrain>();

        cameraEvents.CameraActivatedEvent.AddListener(OnCameraActivated);
    }

    private void OnCameraActivated(ICinemachineMixer mixer, ICinemachineCamera cam) => brain.OutputCamera.cullingMask = targetCullingMask;
}
