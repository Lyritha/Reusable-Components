using UnityEngine;

public class TableSurface : MonoBehaviour
{
    [SerializeField]
    private float hoverOffset = 0.05f;

    [SerializeField]
    private LayerMask tableMask;

    public float HoverOffset => hoverOffset;

    public bool TryGetSurfacePoint(Vector3 worldPos, bool shouldHover, out Vector3 result)
    {
        result = worldPos;
        Ray ray = new(worldPos + Vector3.up, Vector3.down);

        if (!Physics.Raycast(ray, out RaycastHit hit, 5f, tableMask, QueryTriggerInteraction.Ignore)) return false;

        result = hit.point;
        if (shouldHover) result += Vector3.up * hoverOffset;

        return true;
    }
}
