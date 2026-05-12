using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlasticExplosive : MonoBehaviour, IInteractable, IExplodeable
{
    [SerializeField, Header("Explosion settings")]
    private ExplosionSystem explosionSystem;
    [SerializeField]
    private int explosionDelay = 5;

    [SerializeField, Header("Audio")]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip tickSound;


    private Rigidbody rb;
    private bool started = false;
    private Vector3 rayDir = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        explosionSystem.Initialize(gameObject);
        explosionSystem.OnExploded += OnExploded;
    }

    private void OnDisable() => explosionSystem.OnExploded -= OnExploded;
    public void Initialize(Vector3 normal) => rayDir = -normal;


    private void FixedUpdate()
    {
        if (!rb.isKinematic) return;
        bool isKinematic = true;

        Ray ray = new(transform.position, rayDir);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.1f))
        {
            Rigidbody other = hit.collider.attachedRigidbody;
            if (other != null && !other.isKinematic)  isKinematic = false;
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
        float timeLeft = explosionDelay;

        while (timeLeft > 0f)
        {
            float t = 1f - (timeLeft / explosionDelay);

            float tickInterval = Mathf.Lerp(1.0f, 0.1f, t);

            audioSource.pitch = Mathf.Lerp(1f, 2f, t);
            audioSource.PlayOneShot(tickSound);
            audioSource.pitch = 1f;

            yield return new WaitForSeconds(tickInterval);

            timeLeft -= tickInterval;
        }


        StartCoroutine(ExplodeDelayed());
    }

    public void Explode(int _, Vector3 __, float ___, float ____, float _____)
    {
        if (started) return;
        started = true;

        StartCoroutine(ExplodeDelayed());
    }


    private IEnumerator ExplodeDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        explosionSystem.Explode();
    }

    private void OnExploded() => Destroy(gameObject);
}
