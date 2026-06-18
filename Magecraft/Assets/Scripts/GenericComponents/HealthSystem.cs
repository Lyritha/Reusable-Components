using Lyrith.Inspector.ShowOnly;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int maxValue = 100;
    [SerializeField, ShowOnly]
    private int currentValue;

    /// <summary>
    /// Amount restored (e.g., healing or shield regeneration), clamped to max value.
    /// </summary>
    public UnityEvent<int> OnRestored;
    /// <summary>
    /// Amount reduced (e.g., damage taken), clamped to zero.
    /// </summary>
    public UnityEvent<int> OnReduced;
    /// <summary>
    /// Current and maximum value (e.g., health, shield, armor).
    /// </summary>
    public UnityEvent<int, int> OnValueChanged;
    /// <summary>
    /// Triggered when the value reaches zero (e.g., death, shield break).
    /// </summary>
    public UnityEvent OnDepleted;


    private bool isInitialized = false;
    private bool isDepleted = false;

    public int CurrentValue => currentValue;
    public int MaxValue => maxValue;
    public bool IsDepleted => isDepleted;

    private void Awake()
    {
        if (isInitialized) throw new InvalidOperationException("HealthSystem.Initialize() called twice.");

        currentValue = maxValue;
        isInitialized = true;
        isDepleted = false;

        OnValueChanged?.Invoke(currentValue, maxValue);
    }

    public int Restore(int amount)
    {
        if (isDepleted || !isInitialized) return 0;

        int maxAllowed = maxValue - currentValue;
        int healed = Math.Min(amount, maxAllowed);

        currentValue += healed;

        if (healed > 0)
        {
            OnRestored?.Invoke(healed);
            OnValueChanged?.Invoke(currentValue, maxValue);
        }

        return healed;
    }

    public int Reduce(int amount)
    {
        if (isDepleted || !isInitialized) return 0;

        int damage = Math.Max(amount, 0);

        currentValue -= damage;
        OnReduced?.Invoke(damage);

        if (currentValue < 0) currentValue = 0;
        OnValueChanged?.Invoke(currentValue, maxValue);

        if (currentValue == 0)
        {
            isDepleted = true;
            OnDepleted?.Invoke();
        }

        return damage;
    }

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        Reduce(amount);
    }
}
