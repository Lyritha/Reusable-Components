using UnityEngine;
using UnityEngine.InputSystem;

public static class MouseRaycast
{
    private static Camera _camera;

    public static bool TryGetWorldMouseHit(out RaycastHit hit, float range = 100f) => TryGetWorldMouseHit(out hit, ~0, range);
    public static bool TryGetWorldMouseHit(out RaycastHit hit, LayerMask mask, float range = 100f)
    {
        hit = new RaycastHit();

        if (_camera == null) _camera = Camera.main;
        if (_camera == null)
        {
            Debug.LogError("No main camera found.");
            return false;
        }

        if (Pointer.current == null) return false;

        Vector2 screenPos = Pointer.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(screenPos);

        return Physics.Raycast(ray, out hit, range, mask, QueryTriggerInteraction.Ignore);
    }
}
