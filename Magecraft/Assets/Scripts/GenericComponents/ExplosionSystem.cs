using System;
using UnityEngine;

[Serializable]
public class ExplosionSystem
{
    [SerializeField]
    private int damage = 50;
    [SerializeField]
    private float radius = 5;
    [SerializeField, Tooltip("If force = 0, apply no force at all to any object")]
    private float force = 100;
    [SerializeField]
    private GameObject explosionPrefab;

    public Action OnExploded;

    private GameObject gameObject;
    private bool useLineOfSight = true;
    private bool isInitilized = false;
    private bool isExploding = false;

    public void Initialize(GameObject gameObject, bool useLineOfSight = true)
    {
        this.gameObject = gameObject;
        this.useLineOfSight = useLineOfSight;

        isInitilized = true;
        isExploding = false;
    }


    public void Explode()
    {
        if (!isInitilized)
        {
            Debug.LogWarning("Tried to use explosion system, but it has not been initialized yet");
            return;
        }

        if (isExploding) return;
        isExploding = true;

        Vector3 pos = gameObject.transform.position;
        float forceRadius = 2 * radius;

        if (explosionPrefab != null) GameObject.Instantiate(explosionPrefab, pos, Quaternion.identity);

        foreach (Collider col in Physics.OverlapSphere(pos, radius))
        {
            if (col.gameObject == gameObject) continue;
            if (useLineOfSight && !HasLineOfSight(pos, col)) continue;

            if (col.TryGetComponent(out IExplodeable explodeable))
                explodeable.Explode(damage, pos, radius, forceRadius, force);
            else if (col.TryGetComponent(out IDamageable dmg))
                dmg.TakeDamage(damage, pos, (col.transform.position - pos).normalized);

            Rigidbody rb = col.attachedRigidbody;
            bool canApplyForce = rb != null && !rb.isKinematic && force > 0;
            if (canApplyForce) rb.AddExplosionForce(force, pos, forceRadius);
        }

        OnExploded?.Invoke();
    }

    private static bool HasLineOfSight(Vector3 origin, Collider target)
    {
        Vector3 direction = (target.bounds.center - origin).normalized;
        float distance = Vector3.Distance(origin, target.bounds.center);

        foreach (RaycastHit hit in Physics.RaycastAll(origin, direction, distance))
        {
            if (hit.collider == target) return true;
            if (Vector3.Distance(origin, hit.point) < 1f) continue;

            // Ignore explodable objects
            if (hit.collider.TryGetComponent(out IExplodeable _)) continue;

            return false;
        }

        return true;
    }

    public void Gizmo(GameObject obj = null)
    {
        GameObject gizmoObj = gameObject == null ? obj : gameObject;
        if (gizmoObj == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoObj.transform.position, radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gizmoObj.transform.position, radius * 2);
        Gizmos.color = Color.white;
    }
}
