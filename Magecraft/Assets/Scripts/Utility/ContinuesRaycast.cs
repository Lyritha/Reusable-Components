using System;
using UnityEngine;

/// <summary>
/// Call ContinuesRaycast.instance before use to enforce its existence within the scene (otherwise the actions will not trigger)
/// </summary>
public class ContinuesRaycast : MonoBehaviour
{
    [SerializeField]
    private LayerMask raycastLayerMask = ~0;

    private static ContinuesRaycast instance;
    public static Action<RaycastHit> OnRayHit;
    public static Action<RaycastHit> OnRayEntered;
    public static Action OnRayExited;

    private Ray ray = new();
    private Camera cam;

    private Collider hitCollider;

    public static ContinuesRaycast EnsureExistence()
    {
        if (instance != null) return instance;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("ContinuesRaycast: No MainCamera found in scene.");
            return null;
        }

        if (!cam.TryGetComponent(out instance))
            instance = cam.gameObject.AddComponent<ContinuesRaycast>();

        return instance;
    }


    private void Awake()
    {
        // Ensure singleton consistency
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        if (TryRaycast(out RaycastHit hit))
        {
            if (hitCollider != hit.collider) OnRayEntered?.Invoke(hit);

            hitCollider = hit.collider;
            OnRayHit?.Invoke(hit);
        }
        else if (hitCollider != null)
        {
            hitCollider = null;
            OnRayExited?.Invoke();
        }
    }

    private bool TryRaycast(out RaycastHit hit)
    {
        if (cam == null) cam = Camera.main;

        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0f);
        ray = cam.ScreenPointToRay(screenCenter);

        Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayerMask);

        return hit.collider != null;
    }
}
