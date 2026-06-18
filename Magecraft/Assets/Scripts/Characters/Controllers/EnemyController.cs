using Lyrith.Inspector.Fold;
using Lyrith.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : EntityController
{
    public enum EnemyState { Idle, Wandering, TrackingPlayer, AttackingPlayer }

    [SerializeField] private NavAgentPathfinder agent;

    [ Header("Detection")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private LayerMask sightBlockMask;

    [Header("Idle")]
    [SerializeField] private float idleTime = 2f;

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 20f;
    [SerializeField] private float navSampleRadius = 5f;

    [Header("Navigation")]
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float progressCheckInterval = 2f;
    [SerializeField] private float minProgressDistance = 0.5f;

    [SerializeField]
    private EnemyState currentState;

    // tracking
    private Transform player;

    // Idle
    private float idleTimer;

    // Navigation (shared)
    private Vector3 targetPosition;
    private bool hasTarget;
    private float elapsed;
    private float progressCheckElapsed;
    private Vector3 lastCheckedPosition;
    private float distanceCursor;

    private void OnEnable() => agent.RegisterNavAgentObstacle(transform.position);
    private void OnDisable() => agent.DeregisterNavAgentObstacle();

    private void Start() => TransitionTo(EnemyState.Wandering);

    private bool CanSeePlayer()
    {
        // Find player via SphereCast
        if (player == null)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionRadius, Vector3.up, 0f);
            foreach (RaycastHit hit in hits)
                if (hit.collider.TryGetComponent(out PlayerController playerController))
                {
                    player = playerController.transform;
                    break;
                }
        }

        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.sqrMagnitude > detectionRadius * detectionRadius)
        {
            player = null; // player left radius, reset so SphereCast picks them up again
            return false;
        }

        return !Physics.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, sightBlockMask);
    }

    private void TransitionTo(EnemyState next)
    {
        Move.Raise(Vector2.zero, ActiveLayer);
        LookDelta.Raise(Vector2.zero, ActiveLayer);
        PrimaryMouse.Raise(false, ActiveLayer);

        currentState = next;
        switch (currentState)
        {
            case EnemyState.Idle:
                idleTimer = 0f;
                break;
            case EnemyState.Wandering:
            case EnemyState.TrackingPlayer:
                ResetNavigation();
                break;
            case EnemyState.AttackingPlayer:
                PrimaryMouse.Raise(true, ActiveLayer);
                break;
        }
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            if (currentState == EnemyState.AttackingPlayer)
            {
                if ((player.position - transform.position).sqrMagnitude > attackRadius * attackRadius) TransitionTo(EnemyState.TrackingPlayer);
            }
            else if (currentState != EnemyState.TrackingPlayer) TransitionTo(EnemyState.TrackingPlayer);
        }

        switch (currentState)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Wandering: UpdateWander(); break;
            case EnemyState.TrackingPlayer: UpdateTrackPlayer(); break;
            case EnemyState.AttackingPlayer: UpdateAttackPlayer(); break;
        }
    }

    private void FixedUpdate()
    {
        agent.UpdateNavAgentObstacle(transform.position);

        switch (currentState)
        {
            case EnemyState.Wandering:
            case EnemyState.TrackingPlayer:
                FixedUpdateNavigate();
                break;
        }
    }



    // --- Idle ---
    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTime) TransitionTo(EnemyState.Wandering);
    }



    // --- Wander ---
    private void UpdateWander()
    {
        if (!TickNavigationInterval()) return;

        if (agent.Path.HasReachedEnd(transform.position) && hasTarget)
        {
            TransitionTo(EnemyState.Idle);
            return;
        }

        if (!hasTarget)
        {
            hasTarget = false;
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
                Vector3 candidate = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, navSampleRadius, NavMesh.AllAreas))
                {
                    SetTarget(hit.position);
                    break;
                }
            }

            if (!hasTarget) TransitionTo(EnemyState.Idle);
        }

        if (hasTarget) agent.TrySetPath(transform.position, targetPosition);
    }



    // --- Track Player ---
    private void UpdateTrackPlayer()
    {
        if (!CanSeePlayer()) { TransitionTo(EnemyState.Wandering); return; }

        if ((player.position - transform.position).sqrMagnitude <= attackRadius * attackRadius)
        {
            TransitionTo(EnemyState.AttackingPlayer);
            return;
        }

        if (!TickNavigationInterval()) return;
        SetTarget(player.position);
        agent.TrySetPath(transform.position, targetPosition);
    }


    // --- Attack Player ---
    private void UpdateAttackPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Vector2 lookDelta = new(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle), 0f);
        LookDelta.Raise(lookDelta, ActiveLayer);
    }


    // --- Shared Navigation ---
    private void ResetNavigation()
    {
        hasTarget = false;
        elapsed = 0f;
        progressCheckElapsed = 0f;
        distanceCursor = 0f;
        lastCheckedPosition = transform.position;
    }
    private void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
        lastCheckedPosition = transform.position;
    }
    private bool TickNavigationInterval()
    {
        elapsed += Time.deltaTime;
        progressCheckElapsed += Time.deltaTime;

        if (progressCheckElapsed >= progressCheckInterval)
        {
            progressCheckElapsed = 0f;
            if (hasTarget && Vector3.Distance(transform.position, lastCheckedPosition) < minProgressDistance)
                hasTarget = false;
            lastCheckedPosition = transform.position;
        }

        if (elapsed < updateInterval) return false;
        elapsed -= updateInterval;
        return true;
    }
    private void FixedUpdateNavigate()
    {
        if (!agent.Path.HasValidPath || !hasTarget) return;

        HandleWalking(distanceCursor);
        HandleLooking(distanceCursor, 1f);

        if ((agent.Path.GetPositionAtDistance(distanceCursor) - transform.position).sqrMagnitude < 0.0025f) distanceCursor += agent.SampleDistance;
    }



    // --- Movement ---
    private void HandleWalking(float cursor)
    {
        Vector3 dir = (agent.Path.GetPositionAtDistance(cursor) - transform.position).normalized;
        Vector2 local = new(Vector3.Dot(transform.right, dir), Vector3.Dot(transform.forward, dir));
        Move.Raise(local, ActiveLayer);
    }
    private void HandleLooking(float cursor, float lookAhead = 1f)
    {
        Vector3 dir = (agent.Path.GetPositionAtDistance(cursor + lookAhead) - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Vector2 lookDelta = new(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle), 0f);
        LookDelta.Raise(lookDelta, ActiveLayer);
    }



    // --- Gizmos ---
    private void OnDrawGizmos()
    {
        if (agent == null || agent.Path == null || !agent.Path.HasValidPath) return;
        Gizmos.color = Color.yellow;

        NavPath path = agent.Path;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
            Gizmos.DrawSphere(path[i], 0.1f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(path[^1], 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}