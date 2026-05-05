using UnityEngine;

public class Rune : MonoBehaviour, IGrabbable
{
    [SerializeField]
    private TableSurface table;

    [SerializeField]
    private string grabbedLayer;
    private int originalLayer;

    [SerializeField]
    private BulletBehaviour bulletBehaviour;
    public BulletBehaviour BulletBehaviour => bulletBehaviour;

    public bool IsGrabbed { get; private set; }

    public void Initialize(BulletBehaviour behavior)
    {
        bulletBehaviour = behavior;
    }

    private void Awake() => originalLayer = gameObject.layer;

    public void GrabStart(Vector3 grabberPos)
    {
        if (IsGrabbed) return;

        IsGrabbed = true;
        gameObject.layer = LayerMask.NameToLayer(grabbedLayer);
    }

    public void GrabStay(Vector3 grabberPos)
    {
        if (!IsGrabbed) return;

        bool snap = table.TryGetSurfacePoint(grabberPos, true, out Vector3 snapped);
        if (snap) transform.position = snapped;
    }

    public void GrabStop(Vector3 grabberPos)
    {
        if (!IsGrabbed) return;

        IsGrabbed = false;
        gameObject.layer = originalLayer;

        bool snap = table.TryGetSurfacePoint(grabberPos, false, out Vector3 snapped);
        transform.position = snap ? snapped : grabberPos;
    }
}
