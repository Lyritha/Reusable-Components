using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class BreakableWallSegment : MonoBehaviour, IDamageable
{

    // all baked data, set via parent script parentWall.
    [SerializeField]
    private bool requiresSupport = true;
    [SerializeField]
    private List<BreakableWallSegment> neighborsSupportingMe = new();
    [SerializeField]
    private List<BreakableWallSegment> neighborsIAmSupporting = new();


    // runtime data
    public bool IsBroken { get; private set; } = false;

    // cached references
    private BreakableWall parentWall;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        parentWall = GetComponentInParent<BreakableWall>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // entry point for damage from external sources (e.g. gunshots, explosions), pass to parent to handle all fragments together
    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitDirection) => parentWall.TakeDamage(amount, hitPoint);

    // 2 situation where a wall segment should break
    public void ApplyExplosionForce(float force, Vector3 position, float radius)
    {
        if (Vector3.Distance(transform.position, position) <= radius) Break(true, force, position, radius * 3);
    }
    public void TestSupport(BreakableWallSegment caller)
    {
        if (!requiresSupport || IsBroken) return;

        neighborsSupportingMe.Remove(caller);
        if (neighborsSupportingMe.Count == 0) Break(false);
    }

    // handle breaking of this segment, including notifying neighbors and applying explosion force if necessary
    public void Break(bool applyExplosion, float force = 0f, Vector3 pos = default, float radius = 0f)
    {
        if (IsBroken) return;
        IsBroken = true;

        transform.localScale = Vector3.one * 0.9f;

        rb.isKinematic = false;
        if (applyExplosion) rb.AddExplosionForce(force, pos, radius);

        // Notify pieces above me that they lost support
        foreach (BreakableWallSegment upper in neighborsIAmSupporting)
            if (upper != null) upper.TestSupport(this);

        // Notify pieces supporting me that I am breaking, so they can remove me from their lists
        foreach (BreakableWallSegment lower in neighborsSupportingMe)
            if (lower != null) lower.neighborsIAmSupporting.Remove(this);

        // clear reference lists to other fragments.
        neighborsSupportingMe.Clear();
        neighborsIAmSupporting.Clear();

        gameObject.AddComponent<ShootablePhysicsObject>();
        Destroy(this);
    }



    #region Baking data and editor visualization

    public void BakeFragmentData(float materialDensity)
    {
        Bounds b = GetComponent<Collider>().bounds;
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        CalculateMass(b, rb, materialDensity);
        FindNeighborsSupportingMe(b);
    }
    private void CalculateMass(Bounds b, Rigidbody rb, float materialDensity)
    {
        float volume = b.size.x * b.size.y * b.size.z;
        rb.mass = volume * materialDensity;
    }
    private void FindNeighborsSupportingMe(Bounds b)
    {
        // if I'm on the bottom layer, I don't require support
        if (b.min.y <= 0.1f)
        {
            requiresSupport = false;
            return;
        }

        // enforce a minimum search size
        Vector3 minSize = new(0.3f, 0.3f, 0.3f);
        Vector3 halfExtents = Vector3.Max(b.extents * 1.1f, minSize);

        Collider[] hits = Physics.OverlapBox(b.center, halfExtents, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            if (!hit.TryGetComponent(out BreakableWallSegment seg) || seg == this) continue;
            if (seg.transform.position.y > transform.position.y) continue;

            neighborsSupportingMe.Add(seg);
        }

        if (neighborsSupportingMe.Count <= 0) FallbackFindNeighborsSupportingMe(b);
        if (neighborsSupportingMe.Count <= 0) requiresSupport = false;

        // notify other pieces that they are supporting me.
        foreach (BreakableWallSegment seg in neighborsSupportingMe)
            if (seg != null) seg.RegisterNeighborIAmSupporting(this);
    }
    private void FallbackFindNeighborsSupportingMe(Bounds b)
    {
        BreakableWallSegment nearest = null;
        float nearestDist = float.MaxValue;

        Collider[] hitsFallback = Physics.OverlapSphere(b.center, 0.5f, ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hitsFallback)
        {
            if (!hit.TryGetComponent(out BreakableWallSegment seg) || seg == this) continue;
            if (seg.transform.position.y >= transform.position.y - 0.1f) continue;

            float dist = Vector3.SqrMagnitude(seg.transform.position - transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = seg;
            }
        }

        if (nearest != null) neighborsSupportingMe.Add(nearest);
    }


    // handler for neighbors to register themselves as pieces I am supporting,
    // so that when I break I can notify them to test their support as well
    public void RegisterNeighborIAmSupporting(BreakableWallSegment neighbor) => neighborsIAmSupporting.Add(neighbor);

    public void ResetBakedData()
    {
        neighborsIAmSupporting.Clear();
        neighborsSupportingMe.Clear();
        requiresSupport = true;
    }


    // visualize support relationships in editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float hue = GetRandomHueUtility.GetFromId(GetInstanceID());

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        Vector3 myPos = transform.position;

        // Draw pieces supporting me (dark)
        Handles.color = Color.HSVToRGB(hue, 1f, 0.5f);
        Transform[] supporting = neighborsSupportingMe.Select(seg => seg.transform).ToArray();
        DrawLineGroup(myPos, supporting, false);

        // Draw pieces I am supporting (bright)
        Handles.color = Color.HSVToRGB(hue, 1f, 1f);
        Transform[] supported = neighborsIAmSupporting.Select(seg => seg.transform).ToArray();
        DrawLineGroup(myPos, supported, true);

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
    }

    private void DrawLineGroup(Vector3 myPos, Transform[] transforms, bool invert = false)
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform t = transforms[i];
            if (t == null) throw new System.ArgumentException("transforms contains null entries");

            if (invert) GizmoUtils.DrawDirectedLine(t.position, myPos, 0.1f);
            else GizmoUtils.DrawDirectedLine(myPos, t.position, 0.1f);
        }
    }
#endif

    #endregion
}
