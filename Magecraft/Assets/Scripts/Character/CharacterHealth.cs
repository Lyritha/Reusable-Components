using UnityEngine;

public class CharacterHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        currentHealth -= amount;

        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
