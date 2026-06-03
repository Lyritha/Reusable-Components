using System;
using UnityEngine;
using UnityEngine.Events;

public class ExplosiveBarrel : MonoBehaviour, IExplodeable
{
    public UnityEvent OnBarrelExploded = new();

    public void Explode(int _, Vector3 __, float ___, float ____, float _____) => OnBarrelExploded?.Invoke();
}
