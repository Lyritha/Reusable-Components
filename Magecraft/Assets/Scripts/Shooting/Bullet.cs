using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class Bullet
{
    public BulletStats Stats { get; private set; } = BulletStats.Default;

    private readonly List<BulletModifier> allModifiers = new();
    private readonly Dictionary<BulletModifierType ,List<BulletModifier>> sortedModifiers = new();

    public Bullet()
    {
        foreach (BulletModifierType type in Enum.GetValues(typeof(BulletModifierType)))
            sortedModifiers[type] = new List<BulletModifier>();

        Stats = BulletStats.Default;
    }

    public void ExecuteModifiers(GameObject obj, BulletModifierType layer)
    {
        foreach (BulletModifier modifier in sortedModifiers[layer])
            modifier.Execute(obj);
    }

    public void AddModifier(BulletModifier modifier)
    {
        sortedModifiers[modifier.type].Add(modifier);
        allModifiers.Add(modifier);

        Stats.Add(modifier.statModifier);
    }

    public void RemoveModifier(BulletModifier modifier)
    {
        sortedModifiers[modifier.type].Remove(modifier);
        allModifiers.Remove(modifier);

        Stats.Subtract(modifier.statModifier);
    }

    public string GetBulletInfo()
    {
        StringBuilder sb = new();

        // Show only executable modifiers
        foreach (BulletModifier modifier in allModifiers)
            if (modifier.HasExecutable)
                sb.AppendLine($"Modifier: {modifier.BehaviorName}");

        sb.AppendLine();
        AppendIfNonZero(sb, "Damage", Stats.Damage);
        AppendIfNonZero(sb, "Speed", Stats.Speed);
        AppendIfNonZero(sb, "Lifetime", Stats.Lifetime);

        sb.AppendLine();
        AppendIfNonZero(sb, "Pellet Count", Stats.PelletCount);
        AppendIfNonZero(sb, "Pellet Spread", Stats.PelletSpread);
        AppendIfNonZero(sb, "Recoil", Stats.Recoil);

        sb.AppendLine();
        AppendIfNonZero(sb, "Pierce Count", Stats.PierceCount);

        sb.AppendLine();
        AppendIfNonZero(sb, "Knockback", Stats.Knockback);

        sb.AppendLine();
        AppendIfNonZero(sb, "Crit Damage Multiplier", Stats.CritDamageMultiplier);

        return sb.ToString();
    }
    private void AppendIfNonZero(StringBuilder sb, string label, float value)
    {
        if (Math.Abs(value) > 0.0001f) sb.AppendLine($"{label}: {value}");
    }
    private void AppendIfNonZero(StringBuilder sb, string label, int value)
    {
        if (value != 0) sb.AppendLine($"{label}: {value}");
    }
}

