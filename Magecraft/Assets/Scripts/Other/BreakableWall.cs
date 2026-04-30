using NUnit;
using System.Collections.Generic;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BreakableWall : MonoBehaviour
{
    [Header("---- Break Settings ----")]
    [SerializeField] private float breakForce = 250f;
    [SerializeField] private float breakRadius = 3f;
    [SerializeField] private float materialDensity = 50; 

    [Space(10)]
    [Header("---- Fragments ----")]
    [SerializeField, Tooltip("IMPORTANT: will not clear data from segments that have been removed as a child of \"fragmentsParent\".\r\nSo if you remove a segment you will have to manually clear its baked data.")]
    private GameObject fragmentsParent;

    [SerializeField]
    private List<BreakableWallSegment> brokenPieces = new();

    public void TakeDamage(int _, Vector3 hitPoint)
    {
        // First pass: apply explosion force
        for (int i = brokenPieces.Count - 1; i >= 0; i--)
        {
            BreakableWallSegment seg = brokenPieces[i];
            if (seg != null) seg.ApplyExplosionForce(breakForce, hitPoint, breakRadius);
        }

        // Second pass: remove broken pieces
        for (int i = brokenPieces.Count - 1; i >= 0; i--)
        {
            BreakableWallSegment seg = brokenPieces[i];
            if (seg == null || seg.IsBroken) brokenPieces.RemoveAt(i);
        }

        if (brokenPieces.Count == 0)
        {
            Destroy(this);
        }
    }


    // IMPORTANT: will not clear data from segments that have been removed as a child of "fragmentsParent".
    // so if you remove a segment you will have to manually clear its baked data.

    [ContextMenu("Bake Wall Data")]
    public void BakeWallData()
    {
        // clear lists to be repopulated with current children
        brokenPieces.Clear();

        // repopulate brokenPieces with current children and sort by y position (highest first)
        brokenPieces.AddRange(fragmentsParent.GetComponentsInChildren<BreakableWallSegment>());
        brokenPieces.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

        foreach (BreakableWallSegment seg in brokenPieces) seg.ResetBakedData();
        foreach (BreakableWallSegment seg in brokenPieces) seg.BakeFragmentData(materialDensity);

        // avoid changes being lost if user bakes in edit mode
    #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
            foreach (var seg in brokenPieces)
                EditorUtility.SetDirty(seg);
    #endif
    }

    // visualize support relationships in editor
#if UNITY_EDITOR
    private readonly HashSet<(BreakableWallSegment, BreakableWallSegment)> supportPairs = new();
    private readonly Dictionary<BreakableWallSegment, Color> colorCache = new();

    private void OnDrawGizmosSelected()
    {
        // Only draw if THIS object is the one selected
        if (Selection.activeTransform != transform) return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        supportPairs.Clear();
        colorCache.Clear();

        foreach (BreakableWallSegment segA in brokenPieces) 
        {
            if (segA == null) continue;

            // precompute colors
            if (!colorCache.TryGetValue(segA, out Color color))
            {
                float hue = GetRandomHueUtility.GetFromId(segA.GetInstanceID());
                color = Color.HSVToRGB(hue, 1f, 1f);
                colorCache.Add(segA, color);
            }

            // create spheres for all segments
            Handles.color = color;
            Handles.SphereHandleCap(0, segA.transform.position, Quaternion.identity, 0.08f, EventType.Repaint);

            foreach (BreakableWallSegment segB in segA.NeigborsSupportingMe)
            {
                if (segB == null) continue;

                (BreakableWallSegment, BreakableWallSegment) pair = OrderedPair(segA, segB);

                if (supportPairs.Contains(pair)) continue;
                supportPairs.Add(pair);
            }
        }

        foreach ((BreakableWallSegment, BreakableWallSegment) pair in supportPairs)
        {
            Vector3 start = pair.Item1.transform.position;
            Vector3 end = pair.Item2.transform.position;
            Vector3 mid = (start + end) * 0.5f;

            // first half (start → midpoint)
            Handles.color = colorCache[pair.Item1];
            Handles.DrawLine(start, mid);

            // second half (midpoint → end)
            Handles.color = colorCache[pair.Item2];
            Handles.DrawLine(mid, end);
        }

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
    }

    private static (BreakableWallSegment, BreakableWallSegment) OrderedPair(BreakableWallSegment a, BreakableWallSegment b) => a.GetInstanceID() < b.GetInstanceID() ? (a, b) : (b, a);

#endif

}
