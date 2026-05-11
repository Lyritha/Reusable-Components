using System;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable, IExplodeable
{
    [SerializeField]
    private HealthSystem health;

    [SerializeField]
    private GameObject explosionVFXPrefab;

    private void Awake()
    {
        health = new HealthSystem();
        health.Initialize();

        health.OnDepleted += ExplodeBarrel;
    }

    private void OnDisable()
    {
        health.OnDepleted -= ExplodeBarrel;
    }

    public void Explode(int amount, Vector3 position, float sourceRadius, float forceRadius, float force)
    {
        ExplodeBarrel();
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        health.Reduce(amount);
    }

    private void ExplodeBarrel()
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

        Instantiate(explosionVFXPrefab);
        Destroy(gameObject);
    }
}
