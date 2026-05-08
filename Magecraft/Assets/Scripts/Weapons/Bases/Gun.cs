using UnityEngine;

public class Gun : Weapon
{
    protected Ray ray = new();

    protected bool TryRaycast(out RaycastHit hit)
    {
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0f);
        ray = Camera.main.ScreenPointToRay(screenCenter);

        Physics.Raycast(ray, out hit);

        return hit.collider != null;
    }
}
