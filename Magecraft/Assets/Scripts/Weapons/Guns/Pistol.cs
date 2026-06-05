using UnityEngine;

public class Pistol : Gun
{
    [SerializeField]
    private GameObject bulletHitVfx;

    private void Awake()
    {
        AddSubscription(e => e.PrimaryMouse.OnEvent += Use, e => e.PrimaryMouse.OnEvent -= Use);
    }

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

    private void OnDrawGizmos()
    {
        if (ray.direction != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 100f);
        }
    }
}
