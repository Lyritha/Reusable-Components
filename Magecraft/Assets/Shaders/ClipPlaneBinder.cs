using UnityEngine;

[ExecuteAlways]
public class ClipPlaneBinder : MonoBehaviour
{
    [SerializeField]
    private ClipPlane target;
    [SerializeField] private Renderer rend;

    private MaterialPropertyBlock mpb;

    private static readonly int ClipPosID = Shader.PropertyToID("_ClipTargetPos");
    private static readonly int ClipUpID = Shader.PropertyToID("_ClipTargetUp");

    private Vector3 lastPos;
    private Vector3 lastUp;

    private void OnEnable()
    {
        mpb ??= new MaterialPropertyBlock();

        if (target == null) target = GetComponentInParent<ClipPlane>();
        if (rend == null) rend = GetComponent<Renderer>();

        // Immediately apply correct values to avoid 1-frame blink
        if (target != null && rend != null)
        {
            mpb.SetVector(ClipPosID, target.transform.position);
            mpb.SetVector(ClipUpID, target.transform.up);
            rend.SetPropertyBlock(mpb);

            lastPos = target.transform.position;
            lastUp = target.transform.up;
        }
    }

    private void Update()
    {
        if (target == null) target = GetComponentInParent<ClipPlane>();
        if (target == null || rend == null) return;

        Vector3 pos = target.transform.position;
        Vector3 up = target.transform.up;

        // Only update when needed
        if (pos == lastPos && up == lastUp) return;

        lastPos = pos;
        lastUp = up;

        rend.GetPropertyBlock(mpb);
        mpb.SetVector(ClipPosID, pos);
        mpb.SetVector(ClipUpID, up);
        rend.SetPropertyBlock(mpb);
    }
}
