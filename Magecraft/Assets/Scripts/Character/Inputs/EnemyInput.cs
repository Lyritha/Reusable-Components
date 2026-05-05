using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyInput : MonoBehaviour, ICharacterInput
{
    NavMeshAgent agent;

#pragma warning disable CS0067
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    public event Action SwitchLookEvent;
    public event Action<bool> SprintEvent;
    public event Action JumpEvent;
    public event Action<bool> AttackEvent;
    public event Action<int> NumberSelectEvent;
#pragma warning restore CS0067


    private Vector3 targetPosition;
    private float nextPositionTimer = 0;
    private bool wantsToRun = false;

    private float range = 10f;

    private void Awake()
    {
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
            LookEvent?.Invoke(Vector2.zero);
            MoveEvent?.Invoke(Vector2.zero);
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

        for (int i = 0; i < 10; i++) // try up to 10 random points
        {
            Vector3 randomOffset = new(
                Random.Range(-range, range),
                10,
                Random.Range(-range, range)
            );

            Vector3 desired = transform.position + randomOffset;

            // Snap to NavMesh
            if (!NavMesh.SamplePosition(desired, out NavMeshHit hit, 12f, NavMesh.AllAreas)) continue;

            // Validate path
            NavMeshPath path = new();
            if (!NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path)) continue;

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                targetPosition = hit.position;
                agent.SetDestination(targetPosition);
                nextPositionTimer = Random.Range(1f, 3f);
                return;
            }
        }

        // If all attempts fail, wait and try again
        nextPositionTimer = 0.5f;
    }



    private void HandleMovement()
    {
        if (agent.pathPending)
        {
            MoveEvent?.Invoke(Vector2.zero);
            //SprintEvent?.Invoke(false);
            return;
        }

        Vector3 worldDir = (agent.steeringTarget - transform.position).normalized;

        Vector3 localDir = transform.InverseTransformDirection(worldDir);
        Vector2 moveDir = new(localDir.x, localDir.z);

        MoveEvent?.Invoke(moveDir.normalized);
        //SprintEvent?.Invoke(wantsToRun);
    }





    private void HandleLook()
    {
        if (agent.pathPending)
        {
            LookEvent?.Invoke(Vector2.zero);
            return;
        }

        Vector3 toTarget = agent.steeringTarget - transform.position;
        toTarget.y = 0;

        float desiredYaw = Quaternion.LookRotation(toTarget).eulerAngles.y;
        float currentYaw = transform.eulerAngles.y;

        float deltaYaw = Mathf.DeltaAngle(currentYaw, desiredYaw) * 0.25f;

        LookEvent?.Invoke(new Vector2(deltaYaw, 0));
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
