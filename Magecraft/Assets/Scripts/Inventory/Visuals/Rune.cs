using UnityEngine;

public class Rune : MonoBehaviour, IGrabbable
{
    [SerializeField]
    private TableSurface table;

    [SerializeField]
    private string grabbedLayer;
    private int originalLayer;

    [SerializeField]
    private BulletModifier modifier;
    public BulletModifier Modifier => modifier;

    public bool IsGrabbed { get; private set; }

    // Local-space target instead of world-space
    private Vector3 targetLocalPos;


    public void Initialize(BulletModifier modifier)
    {
        this.modifier = modifier;
    }

    private void Awake()
    {
        originalLayer = gameObject.layer;
        targetLocalPos = transform.localPosition;
    }

    public void GrabStart(Vector3 grabberPos)
    {
        if (IsGrabbed) return;

        IsGrabbed = true;
        gameObject.layer = LayerMask.NameToLayer(grabbedLayer);
    }

    public void GrabStay(Vector3 grabberPos)
    {
        if (!IsGrabbed) return;

        if (table.TryGetSurfacePoint(grabberPos, true, out Vector3 snapped))
        {
            // Convert world → local
            targetLocalPos = transform.parent.InverseTransformPoint(snapped);
        }
    }

    public void GrabStop(Vector3 grabberPos)
    {
        if (!IsGrabbed) return;

        IsGrabbed = false;
        gameObject.layer = originalLayer;

        bool snap = table.TryGetSurfacePoint(grabberPos, false, out Vector3 snapped);
        Vector3 worldTarget = snap ? snapped : grabberPos;

        // Convert world → local
        targetLocalPos = transform.parent.InverseTransformPoint(worldTarget);
    }

    private void Update()
    {
        Vector3 pos = transform.localPosition;

        float speed = Time.deltaTime * (Vector3.Distance(pos, targetLocalPos) * 30f);
        speed = Mathf.Max(speed, Time.deltaTime);

        transform.localPosition = Vector3.MoveTowards(pos, targetLocalPos, speed);
    }
}
