using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("---- materialSettings ----")]
    [SerializeField] private float materialDensity = 50;
    [SerializeField] private float minMaterialDensity = 3; 

    [Space(10)]
    [Header("---- Fragments ----")]
    [SerializeField, Tooltip("IMPORTANT: will not clear data from segments that have been removed as a child of \"fragmentsParent\".\r\nSo if you remove a segment you will have to manually clear its baked data.")]
    private GameObject fragmentsParent;

    [SerializeField]
    private List<BreakableWallSegment> brokenPieces = new();

    public void OnPieceBroken()
    {
        // Second pass: remove broken pieces
        for (int i = brokenPieces.Count - 1; i >= 0; i--)
        {
            BreakableWallSegment seg = brokenPieces[i];
            if (seg == null || seg.IsBroken) brokenPieces.RemoveAt(i);
        }

        if (brokenPieces.Count == 0) Destroy(this);
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
        foreach (BreakableWallSegment seg in brokenPieces) seg.BakeFragmentData(minMaterialDensity, materialDensity);

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

        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) return;

        Camera cam = sceneView.camera;
        Vector3 camPos = cam.transform.position;

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
            float dist = Vector3.Distance(camPos, segA.transform.position);
            if (dist < 15)
            {
                Handles.color = color;
                Handles.SphereHandleCap(0, segA.transform.position, Quaternion.identity, 0.05f, EventType.Repaint);
            }

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
            if (pair.Item1 == null || pair.Item2 == null) continue;

            Vector3 start = pair.Item1.transform.position;
            Vector3 end = pair.Item2.transform.position;
            Vector3 mid = (start + end) * 0.5f;

            float dist = Vector3.Distance(camPos, mid);
            float sqrLen = (end - start).sqrMagnitude;

            // Medium distance → hide very short connections
            if (dist > 5f && sqrLen < 0.3f) continue;
            if (dist > 10f) continue;

            // --- Draw two-color line ---
            Handles.color = colorCache[pair.Item1];
            Handles.DrawLine(start, mid);

            Handles.color = colorCache[pair.Item2];
            Handles.DrawLine(mid, end);
        }


        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
    }

    private static (BreakableWallSegment, BreakableWallSegment) OrderedPair(BreakableWallSegment a, BreakableWallSegment b) => a.GetInstanceID() < b.GetInstanceID() ? (a, b) : (b, a);

#endif

}
