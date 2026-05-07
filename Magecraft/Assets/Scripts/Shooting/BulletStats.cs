using System;
using UnityEngine;

[Serializable]
public class BulletStats
{
    [Header("Core")]
    public int Damage = 0;
    public float Speed = 0;
    public float Lifetime = 0;

    [Header("Multi-shot")]
    public int PelletCount = 0;
    public float PelletSpread = 0;
    public float Recoil = 0;

    [Header("Penetration")]
    public int PierceCount = 0;

    [Header("Knockback")]
    public float Knockback = 0;

    [Header("Scaling")]
    public float CritDamageMultiplier = 0f;

    public static BulletStats Default => new()
    {
        Damage = 10,
        Speed = 5,
        Lifetime = 5f,

        PelletCount = 1,
        PelletSpread = 0,

        CritDamageMultiplier = 1.1f,
    };



    public void Add(BulletStats other)
    {
        Damage += other.Damage;
        Speed += other.Speed;
        Lifetime += other.Lifetime;

        PelletCount += other.PelletCount;
        PelletSpread += other.PelletSpread;
        Recoil += other.Recoil;

        PierceCount += other.PierceCount;

        Knockback += other.Knockback;

        CritDamageMultiplier += other.CritDamageMultiplier;
    }

    public void Subtract(BulletStats other)
    {
        Damage -= other.Damage;
        Speed -= other.Speed;
        Lifetime -= other.Lifetime;

        PelletCount -= other.PelletCount;
        PelletSpread -= other.PelletSpread;
        Recoil -= other.Recoil;

        PierceCount -= other.PierceCount;

        Knockback -= other.Knockback;

        CritDamageMultiplier -= other.CritDamageMultiplier;
    }


}

