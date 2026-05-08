using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float smoothing = 0.5f;

    public Transform Target { get { return target; } }

    Ray ray;
    private float lastValidRayDistance = 3;

    private Vector3 targetPoint = Vector3.zero;

    private void Update()
    {
        if (TryRaycast(out RaycastHit hit)) { 
            targetPoint = hit.point; 
            lastValidRayDistance = hit.distance;
        }
        else targetPoint = ray.GetPoint(lastValidRayDistance);

            target.position = Vector3.MoveTowards(
                target.position,
                targetPoint,
                smoothing);
    }

    private bool TryRaycast(out RaycastHit hit)
    {
        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0f);
        ray = Camera.main.ScreenPointToRay(screenCenter);

        Physics.Raycast(ray, out hit);

        return hit.collider != null;
    }
}
