using Lyrith.Navigation;
using System.IO;
using UnityEngine;

public class NavMeshFollower : MonoBehaviour
{
    [SerializeField]
    GameObject follower;
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private NavAgentPathfinder agent;

    private float elapsed = 0.0f;

    private void OnEnable()
    {
        agent.RegisterNavAgentObstacle(follower.transform.position);
    }

    private void OnDisable()
    {
        agent.DeregisterNavAgentObstacle();
    }

    void Start()
    {
        elapsed = 0.0f;
    }

    [SerializeField]
    float speed = 5f;

    public float distanceCursor;

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 0.1f)
        {
            elapsed -= 0.1f;
            agent.TrySetPath(follower.transform.position, target.position);
        }
    }

    private Vector3 currentTarget = Vector3.zero;

    void FixedUpdate()
    {
        if (!agent.Path.HasValidPath) return;

        // 1. Compute the current target point on the path
        Vector3 targetPoint = agent.Path.GetPositionAtDistance(distanceCursor);
        currentTarget = targetPoint;

        // 2. Move physics body toward the target
        Vector3 toTarget = targetPoint - follower.transform.position;
        Vector3 desiredVel = toTarget.normalized * speed;

        rb.linearVelocity = desiredVel; // or AddForce, or MovePosition

        // 3. Only advance the cursor if we actually reached the target
        if (toTarget.sqrMagnitude < 0.0025f) distanceCursor += agent.SampleDistance;

        agent.UpdateNavAgentObstacle(follower.transform.position);
    }




    private void OnDrawGizmos()
    {
        if (follower != null && target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(follower.transform.position, target.position);
        }

        if (agent == null || agent.Path == null || !agent.Path.HasValidPath) return;
        Gizmos.color = Color.yellow;

        NavPath path = agent.Path;

        for (int i = 0; i < agent.Path.Length - 1; i++)
        {
            Vector3 pos = path.Waypoints[i].Position;

            Gizmos.DrawLine(path[i], path[i + 1]);
            Gizmos.DrawSphere(path[i], 0.1f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(path[^1], 0.5f);

        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(currentTarget, .5f);
    }
}
