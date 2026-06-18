using Lyrith.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : EntityController
{
    [SerializeField] private NavAgentPathfinder agent;

    private float elapsed = 0.0f;

    private void OnEnable() => agent.RegisterNavAgentObstacle(transform.position);
    private void OnDisable() => agent.DeregisterNavAgentObstacle();
    private void Start() => elapsed = 0.0f;

    public float distanceCursor;
    public Vector3 targetPosition = Vector3.zero;
    public bool hasTarget = false;

    // Progress tracking
    private Vector3 lastCheckedPosition;
    private float progressCheckElapsed = 0f;
    private const float ProgressCheckInterval = 2f;
    private const float MinProgressDistance = 0.5f;

    void Update()
    {
        elapsed += Time.deltaTime;
        progressCheckElapsed += Time.deltaTime;

        // Progress check
        if (progressCheckElapsed >= ProgressCheckInterval)
        {
            progressCheckElapsed = 0f;

            float distanceMoved = Vector3.Distance(transform.position, lastCheckedPosition);
            if (hasTarget && distanceMoved < MinProgressDistance) hasTarget = false; // stalled — force new target

            lastCheckedPosition = transform.position;
        }

        if (elapsed < 0.1f) return;
        elapsed -= 0.1f;

        if (agent.Path.HasReachedEnd(transform.position) || !hasTarget)
        {
            hasTarget = false;

            const int maxAttempts = 10;
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * 20f;
                Vector3 candidate = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                    hasTarget = true;
                    lastCheckedPosition = transform.position; // reset baseline on new target
                    break;
                }
            }
        }

        agent.TrySetPath(transform.position, targetPosition);
    }

    void FixedUpdate()
    {
        if (!agent.Path.HasValidPath || !hasTarget) return;

        HandleWalking();
        HandleLooking(1);

        if (IsCloseToTarget()) distanceCursor += agent.SampleDistance;
        agent.UpdateNavAgentObstacle(transform.position);
    }

    private void HandleWalking()
    {
        Vector3 normalizedDirection = (agent.Path.GetPositionAtDistance(distanceCursor) - transform.position).normalized;
        Vector2 localDirection = new(Vector3.Dot(transform.right, normalizedDirection), Vector3.Dot(transform.forward, normalizedDirection));
        Move.Raise(localDirection, ActiveLayer);
    }

    private void HandleLooking(float lookAhead = 1f)
    {
        Vector3 normalizedDirection = (agent.Path.GetPositionAtDistance(distanceCursor + lookAhead) - transform.position).normalized;
        float targetAngle = Mathf.Atan2(normalizedDirection.x, normalizedDirection.z) * Mathf.Rad2Deg;
        Vector2 lookDelta = new(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle), 0f);
        LookDelta.Raise(lookDelta, ActiveLayer);
    }

    private bool IsCloseToTarget() => (agent.Path.GetPositionAtDistance(distanceCursor) - transform.position).sqrMagnitude < 0.0025f;

    private void OnDrawGizmos()
    {
        if (agent == null || agent.Path == null || !agent.Path.HasValidPath) return;
        Gizmos.color = Color.yellow;

        NavPath path = agent.Path;
        for (int i = 0; i < agent.Path.Length - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
            Gizmos.DrawSphere(path[i], 0.1f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(path[^1], 0.5f);
    }
}