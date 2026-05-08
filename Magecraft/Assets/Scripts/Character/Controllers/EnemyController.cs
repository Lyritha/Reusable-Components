using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : EntityController
{
    private NavMeshAgent agent;

    private Vector3 targetPosition;
    private float nextPositionTimer = 0f;

    private float range = 10f;

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        GetNewPosition();
    }

    private void Update()
    {
        if (HasReachedPathEnd())
        {
            RaiseLookDelta(Vector2.zero);
            RaiseMove(Vector2.zero);
            GetNewPosition();
        }
        else
        {
            HandleLook();
            HandleMovement();
        }
    }

    private void GetNewPosition()
    {
        if ((nextPositionTimer -= Time.deltaTime) > 0) return;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomOffset = new(
                Random.Range(-range, range),
                10,
                Random.Range(-range, range)
            );

            Vector3 desired = transform.position + randomOffset;

            if (!NavMesh.SamplePosition(desired, out NavMeshHit hit, 12f, NavMesh.AllAreas))
                continue;

            NavMeshPath path = new();
            if (!NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                continue;

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                targetPosition = hit.position;
                agent.SetDestination(targetPosition);
                nextPositionTimer = Random.Range(1f, 3f);
                return;
            }
        }

        nextPositionTimer = 0.5f;
    }

    private void HandleMovement()
    {
        if (agent.pathPending)
        {
            RaiseMove(Vector2.zero);
            return;
        }

        Vector3 worldDir = (agent.steeringTarget - transform.position).normalized;

        Vector3 localDir = transform.InverseTransformDirection(worldDir);
        Vector2 moveDir = new(localDir.x, localDir.z);

        RaiseMove(moveDir.normalized);
    }

    private void HandleLook()
    {
        if (agent.pathPending)
        {
            RaiseLookDelta(Vector2.zero);
            return;
        }

        Vector3 toTarget = agent.steeringTarget - transform.position;
        toTarget.y = 0;

        float desiredYaw = Quaternion.LookRotation(toTarget).eulerAngles.y;
        float currentYaw = transform.eulerAngles.y;

        float deltaYaw = Mathf.DeltaAngle(currentYaw, desiredYaw) * 0.25f;

        RaiseLookDelta(new Vector2(deltaYaw, 0));
    }

    private bool HasReachedPathEnd()
    {
        return !agent.hasPath || agent.remainingDistance <= agent.stoppingDistance;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.2f);
        }
    }
}
