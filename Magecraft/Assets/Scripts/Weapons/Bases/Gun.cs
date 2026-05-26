using UnityEngine;

public abstract class Gun : Weapon
{
    [SerializeField]
    private LayerMask rayMask = ~0;
    protected Ray ray = new();

    protected bool TryRaycast(out RaycastHit hit)
    {
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0f);
        ray = Camera.main.ScreenPointToRay(screenCenter);

        Physics.Raycast(ray, out hit, 1000, rayMask);

        return hit.collider != null;
    }
}
