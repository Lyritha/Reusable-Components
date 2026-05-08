using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private HealthSystem health = new();
    public HealthSystem Health => health;

    private void Awake()
    {
        health.OnDepleted += Die;
        if (healthSlider != null ) health.OnValueChanged += UpdateUI;

        health.Initialize();
    }

    private void OnDestroy()
    {
        health.OnDepleted -= Die;
        if (healthSlider != null) health.OnValueChanged -= UpdateUI;
    }

    private void UpdateUI(int currentHealth, int maxHealth)
    {
        if ( healthSlider != null )
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int amount, Vector3 _, Vector3 __) => health.Reduce(amount);

    public void Die() => Destroy(gameObject);
}
