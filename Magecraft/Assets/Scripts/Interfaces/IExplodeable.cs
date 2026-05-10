using UnityEngine;

public interface IExplodeable
{
    void Explode(int amount, Vector3 position, float sourceRadius, float forceRadius, float force);
}
