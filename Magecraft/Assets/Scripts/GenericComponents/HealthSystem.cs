using System;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class HealthSystem
{
    [SerializeField]
    private int maxValue = 100;
    [SerializeField, ShowOnly]
    private int currentValue;

    /// <summary>
    /// Amount restored (e.g., healing or shield regeneration), clamped to max value.
    /// </summary>
    public Action<int> OnRestored;
    /// <summary>
    /// Amount reduced (e.g., damage taken), clamped to zero.
    /// </summary>
    public Action<int> OnReduced;
    /// <summary>
    /// Current and maximum value (e.g., health, shield, armor).
    /// </summary>
    public Action<int, int> OnValueChanged;
    /// <summary>
    /// Triggered when the value reaches zero (e.g., death, shield break).
    /// </summary>
    public Action OnDepleted;


    private bool isInitialized = false;
    private bool isDepleted = false;

    public int CurrentValue => currentValue;
    public int MaxValue => maxValue;
    public bool IsDepleted => isDepleted;

    public void Initialize()
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
}
