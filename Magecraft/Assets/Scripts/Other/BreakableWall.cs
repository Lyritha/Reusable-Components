using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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

}
