using UnityEngine;

public class GunAutomatic : Gun
{
    [SerializeField]
    private GameObject bulletHitVfx;

    private bool isFiring = false;
    private float fireRate = 0.1f;
    private float fireTimer;

    private void Awake()
    {
        AddSubscription(e => e.OnPrimaryMouse.OnEvent += Use, e => e.OnPrimaryMouse.OnEvent -= Use);
    }

    public void Use(bool Started) => isFiring = Started;

    protected override void Update()
    {
        base.Update();

        if (isFiring)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer < fireRate) return;

            if (TryRaycast(out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(10, hit.point, ray.direction);

                if (bulletHitVfx != null)
                    Instantiate(bulletHitVfx, hit.point, Quaternion.LookRotation(hit.normal));
            }

            fireTimer = 0;
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
