using UnityEngine;

public class Gun : MonoBehaviour, IWeapon
{
    [SerializeField]
    private GameObject bulletHitVfx;

    Ray ray = new();

    public void Use(bool Started)
    {
        if (!Started) return;

        if (TryRaycast(out RaycastHit hit))
        {
            if (hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(10, hit.point, ray.direction);

            if (bulletHitVfx != null)
                Instantiate(bulletHitVfx, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    private bool TryRaycast(out RaycastHit hit)
    {
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0f);
        ray = Camera.main.ScreenPointToRay(screenCenter);

        Physics.Raycast(ray, out hit);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (ray.direction != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 100f);
        }
    }
}
