using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlasticExplosive : MonoBehaviour, IExplodeable
{
    private static WaitForSeconds _waitForSeconds0_25 = new(0.25f);

    [SerializeField, Header("Audio")]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip tickSound;

    public UnityEvent OnExploded = new();
    private Coroutine tickingRoutine;

    private Rigidbody rb;
    private bool started = false;
    private Vector3 rayDir = Vector3.zero;

    private void Awake() => rb = GetComponent<Rigidbody>();

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

    public void OnCountdownProgress(float normalized)
    {
        // Start ticking when countdown begins
        tickingRoutine ??= StartCoroutine(TickingRoutine());
        currentTickInterval = Mathf.Lerp(0.8f, 0.05f, 1f - normalized);
    }

    private float currentTickInterval = 0.8f;

    private IEnumerator TickingRoutine()
    {
        while (true)
        {
            audioSource.PlayOneShot(tickSound);
            yield return new WaitForSeconds(currentTickInterval);
        }
    }

    public void OnCountdownEnded()
    {
        if (tickingRoutine != null) StopCoroutine(tickingRoutine);
    }


    public void Explode(int _, Vector3 __, float ___, float ____, float _____)
    {
        if (started) return;
        started = true;

        StartCoroutine(ExplodeDelayed());
    }


    private IEnumerator ExplodeDelayed()
    {
        yield return _waitForSeconds0_25;
        OnExploded?.Invoke();
    }
}
