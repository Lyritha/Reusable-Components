using System;
using UnityEngine;

[Serializable]
public class ExplosionSystem
{
    [SerializeField]
    private float explosionRadius = 5;
    [SerializeField]
    private int explosionDamage = 50;
    [SerializeField]
    private float explosionForce = 100;

    private Transform transform;
    private GameObject gameObject;

    public void Initialize(Transform transform, GameObject gameObject)
    {
        this.transform = transform;
        this.gameObject = gameObject;
    }


    public void Explode()
    {
        foreach (Collider col in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            if (col.gameObject == gameObject) continue;

            if (!HasLineOfSight(transform.position, col)) continue;

            Rigidbody rb = col.attachedRigidbody;
            bool canApplyForce = rb != null && !rb.isKinematic;
            float forceRadius = 2 * explosionRadius;

            if (col.TryGetComponent(out IExplodeable explodeable))
            {
                explodeable.Explode(explosionDamage, transform.position, explosionRadius, forceRadius, explosionForce);

                if (canApplyForce) col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, forceRadius);
                continue;
            }

            if (col.TryGetComponent(out IDamageable dmg))
            {
                Vector3 hitDirection = (col.transform.position - transform.position).normalized;
                dmg.TakeDamage(explosionDamage, transform.position, hitDirection);

                if (canApplyForce) col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, forceRadius);
                continue;
            }

            if (canApplyForce) col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, forceRadius);
        }
    }

    private static bool HasLineOfSight(Vector3 origin, Collider target)
    {
        Vector3 direction = (target.bounds.center - origin).normalized;
        float distance = Vector3.Distance(origin, target.bounds.center);

        // Get ALL hits along the ray
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);

        foreach (var hit in hits)
        {
            // Ignore the explosive itself
            if (hit.collider == target) return true;

            // Allow some penetration into obstacles
            if (Vector3.Distance(origin, hit.point) < 1f) continue;

            // Ignore explodable objects (they should not block LOS)
            if (hit.collider.TryGetComponent(out IExplodeable _)) continue;

            // Anything else blocks LOS
            return false;
        }

        return true;
    }
}
