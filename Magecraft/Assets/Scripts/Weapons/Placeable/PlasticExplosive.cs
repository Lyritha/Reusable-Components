using System.Collections;
using UnityEngine;

public class PlasticExplosive : MonoBehaviour, IInteractable, IExplodeable
{
    [SerializeField]
    private int explosionTime = 5;
    [SerializeField]
    private float explosionRadius = 5;
    [SerializeField]
    private int explosionDamage = 50;
    [SerializeField]
    private float explosionForce = 100;

    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    private AudioSource AudioSource;
    [SerializeField]
    private AudioClip tickSound;

    private Rigidbody rb;

    private bool started = false;
    private bool isExploding = false;
    private Vector3 rayDir = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 normal)
    {
        rayDir = -normal;    
    }

    private void FixedUpdate()
    {
        if (!rb.isKinematic) return;
        bool isKinematic = true;

        Ray ray = new(transform.position, rayDir);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.1f))
        {
            Rigidbody rb2 = hit.collider.attachedRigidbody;
            if (rb2 != null && !rb2.isKinematic)  isKinematic = false;
        }

        rb.isKinematic = isKinematic;
    }

    public void Interact()
    {
        if (started) return;

        started = true;
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        float timeLeft = explosionTime;

        while (timeLeft > 0f)
        {
            float t = 1f - (timeLeft / explosionTime);

            float tickInterval = Mathf.Lerp(1.0f, 0.1f, t);

            AudioSource.pitch = Mathf.Lerp(1f, 2f, t);
            AudioSource.PlayOneShot(tickSound);
            AudioSource.pitch = 1f;

            yield return new WaitForSeconds(tickInterval);

            timeLeft -= tickInterval;
        }

        Explode();
    }

    private void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        StartCoroutine(ExplodeDelayed());
    }
    public void Explode(int amount, Vector3 position, float sourceRadius, float forceRadius, float force)
    {
        if (isExploding) return;
        isExploding = true;

        StartCoroutine(ExplodeDelayed());
    }


    private IEnumerator ExplodeDelayed()
    {
        yield return new WaitForSeconds(0.25f);

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        foreach (Collider col in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            if (col.gameObject == gameObject) continue;

            if (!HasLineOfSight(transform.position, col)) continue;

            Rigidbody rb = col.attachedRigidbody;
            bool canApplyForce = rb != null && !rb.isKinematic;
            float forceRadius = 2 * explosionRadius;

            if (col.TryGetComponent(out IExplodeable explodeable))
            {
                explodeable.Explode(explosionDamage, transform.position, explosionRadius, forceRadius, explosionForce);

                if (canApplyForce) col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, forceRadius);
                continue;
            }

            if (col.TryGetComponent(out IDamageable dmg))
            {
                Vector3 hitDirection = (col.transform.position - transform.position).normalized;
                dmg.TakeDamage(explosionDamage, transform.position, hitDirection);

                if (canApplyForce) col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, forceRadius);
                continue;
            }

            if (canApplyForce) col.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, forceRadius);
        }

        Destroy(gameObject);
        yield return null;
    }

    private bool HasLineOfSight(Vector3 origin, Collider target)
    {
        Vector3 direction = (target.bounds.center - origin).normalized;
        float distance = Vector3.Distance(origin, target.bounds.center);

        // Get ALL hits along the ray
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);

        foreach (var hit in hits)
        {
            // Ignore the explosive itself
            if (hit.collider == target) return true;

            // Allow some penetration into obstacles
            if (Vector3.Distance(origin, hit.point) < 1f) continue;

            // Ignore explodable objects (they should not block LOS)
            if (hit.collider.TryGetComponent(out IExplodeable _)) continue;

            // Anything else blocks LOS
            return false;
        }

        return true;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius * 2);
    }
}
