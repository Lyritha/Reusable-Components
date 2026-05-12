using System;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable, IExplodeable
{
    [SerializeField]
    private HealthSystem health;
    [SerializeField, Header("Explosion settings")]
    private ExplosionSystem explosionSystem;

    private void Awake()
    {
        health.Initialize();
        explosionSystem.Initialize(gameObject);

        health.OnDepleted += explosionSystem.Explode;
        explosionSystem.OnExploded += OnExploded;
    }

    private void OnDisable()
    {
        health.OnDepleted -= explosionSystem.Explode;
        explosionSystem.OnExploded -= OnExploded;
    }

    public void Explode(int _, Vector3 __, float ___, float ____, float _____) => explosionSystem.Explode();
    public void TakeDamage(int amount, Vector3 _, Vector3 __) => health.Reduce(amount);
    private void OnExploded() => Destroy(gameObject);

    private void OnDrawGizmosSelected() => explosionSystem.Gizmo(gameObject);
}
