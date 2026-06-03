using UnityEngine;
using UnityEngine.Events;

public class ExplosionSystem : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private int damage = 50;
    [SerializeField] private float radius = 5f;
    [SerializeField, Tooltip("If force = 0, no force will be applied to any object.")]
    private float force = 100f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private bool useLineOfSight = true;

    [Header("Events")]
    public UnityEvent ExplosionStarted = new();
    public UnityEvent ExplosionFinished = new();

    private bool exploding = false;

    public void TriggerExplosion()
    {
        if (exploding) return;
        exploding = true;

        Vector3 origin = transform.position;
        float forceRadius = radius * 2f;

        ExplosionStarted?.Invoke();

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, origin, Quaternion.identity);

        foreach (Collider col in Physics.OverlapSphere(origin, radius))
        {
            if (col.gameObject == gameObject) continue;
            if (useLineOfSight && !HasLineOfSight(origin, col)) continue;

            if (col.TryGetComponent(out IExplodeable explodeable))
            {
                explodeable.Explode(damage, origin, radius, forceRadius, force);
            }
            else if (col.TryGetComponent(out IDamageable damageable))
            {
                Vector3 direction = (col.transform.position - origin).normalized;
                damageable.TakeDamage(damage, origin, direction);
            }

            Rigidbody rb = col.attachedRigidbody;
            bool canApplyForce = rb != null && !rb.isKinematic && force > 0f;

            if (canApplyForce) rb.AddExplosionForce(force, origin, forceRadius);
        }

        ExplosionFinished?.Invoke();
    }

    private static bool HasLineOfSight(Vector3 origin, Collider target)
    {
        Vector3 direction = (target.bounds.center - origin).normalized;
        float distance = Vector3.Distance(origin, target.bounds.center);

        foreach (RaycastHit hit in Physics.RaycastAll(origin, direction, distance))
        {
            if (hit.collider == target) return true;
            if (Vector3.Distance(origin, hit.point) < 1f) continue;
            if (hit.collider.TryGetComponent(out IExplodeable _)) continue;

            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, radius * 2f);

        Gizmos.color = Color.white;
    }
}
