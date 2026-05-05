using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField]
    private HealthSystem health = new();
    public HealthSystem Health => health;

    private void Awake()
    {
        health.Initialize();
        health.OnDepleted += Die;
    }

    private void OnDestroy()
    {
        health.OnDepleted -= Die;
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        health.Reduce(amount);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
