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

    private Vector3 targetPos;


    public void Initialize(BulletModifier modifier)
    {
        this.modifier = modifier;
    }

    private void Awake()
    {
        originalLayer = gameObject.layer;
        targetPos = transform.position;
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

        bool snap = table.TryGetSurfacePoint(grabberPos, true, out Vector3 snapped);
        if (snap) targetPos = snapped;
    }

    public void GrabStop(Vector3 grabberPos)
    {
        if (!IsGrabbed) return;

        IsGrabbed = false;
        gameObject.layer = originalLayer;

        bool snap = table.TryGetSurfacePoint(grabberPos, false, out Vector3 snapped);
        targetPos = snap ? snapped : grabberPos;
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        float speed = Time.deltaTime * (Vector3.Distance(pos, targetPos) * 30);
        speed = Mathf.Max(speed, Time.deltaTime);

        transform.position = Vector3.MoveTowards(pos, targetPos, speed);
    }
}
