using UnityEngine;
using UnityEngine.AI;

public class NavMeshFollower : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private string agentName = "Humanoid";

    private NavMeshPath pathData;
    private float elapsed = 0.0f;
    private NavPath path;
    private int agentTypeID;

    void Start()
    {
        agentTypeID = GetAgentTypeIDByName(agentName);
        pathData = new NavMeshPath();
        elapsed = 0.0f;
    }

    void Update()
    {
        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if (elapsed > .1f)
        {
            elapsed -= .1f;
            NavMeshQueryFilter filter = new NavMeshQueryFilter
            {
                agentTypeID = agentTypeID,
                areaMask = NavMesh.AllAreas
            };

            NavMesh.CalculatePath(transform.position, target.position, filter, pathData);
            path = pathData.corners;
            path = path.PushAwayFromWalls().CleanPath().SmoothPath();
        }
    }


    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }

        if (path == null || path.Length <= 0) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < path.Length - 1; i++)
        {

            Gizmos.DrawLine(path[i], path[i + 1]);
            Gizmos.DrawSphere(path[i], 0.1f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(path[^1], 0.5f);
    }

    public static int GetAgentTypeIDByName(string agentTypeName)
    {
        int count = NavMesh.GetSettingsCount();
        for (int i = 0; i < count; i++)
        {
            var settings = NavMesh.GetSettingsByIndex(i);
            string name = NavMesh.GetSettingsNameFromID(settings.agentTypeID);
            if (name == agentTypeName)
                return settings.agentTypeID;
        }
        return -1; // not found
    }
}
