using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TransformLayout : MonoBehaviour
{
    [Header("Layout Settings")]
    private LayoutMode mode = LayoutMode.Horizontal;
    [SerializeField]
    private AnchorPosition anchorPosition;
    [SerializeField]
    private float spacing = 0.5f;
    [SerializeField]
    private bool dynamicSpacing = false;

    [SerializeField]
    private Vector3 boundsSize = new(5f, 5f, 5f);
    [SerializeField]
    private Vector3 boundsOffset = new(0f, 0f, 0f);

    private readonly List<Transform> _children = new();

    private void OnValidate() => ApplyLayout();

#if UNITY_EDITOR 
    private void OnTransformChildrenChanged() => EditorApplication.delayCall += ApplyLayout;
#endif

    public void ApplyLayout()
    {
        // try to get all children again
        _children.Clear();
        foreach (Transform child in transform) 
            _children.Add(child);

        if (_children.Count == 0) return;

        switch (mode)
        {
            case LayoutMode.Horizontal:
                if (dynamicSpacing) LayoutHorizontalDynamic();
                else LayoutHorizontal();
                break;
        }
    }


    private void LayoutHorizontal()
    {
        Vector3 anchorPosition = GetAnchorPoint();


        float totalWidth = (_children.Count - 1) * spacing;
        float startX = anchorPosition.x - totalWidth * 0.5f;

        for (int i = 0; i < _children.Count; i++)
        {
            Transform child = _children[i];

            Vector3 pos = child.localPosition;
            pos.x = startX + i * spacing;
            pos.y = anchorPosition.y;
            pos.z = anchorPosition.z;

            child.localPosition = pos;
        }
    }

    private void LayoutHorizontalDynamic()
    {
        Vector3 anchorPosition = GetAnchorPoint();

        float totalWidth = 0f;

        // 1. Sum local widths
        for (int i = 0; i < _children.Count; i++)
        {
            Transform child = _children[i];

            if (child.TryGetComponent<MeshRenderer>(out var meshRenderer))
                totalWidth += meshRenderer.localBounds.size.x;
        }

        totalWidth += (_children.Count - 1) * spacing;

        // 2. Center around anchor
        float startX = anchorPosition.x - totalWidth * 0.5f;

        float currentX = startX;

        // 3. Position children using local widths
        for (int i = 0; i < _children.Count; i++)
        {
            Transform child = _children[i];

            if (child.TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                float width = meshRenderer.localBounds.size.x;

                Vector3 pos = child.localPosition;
                pos.x = currentX + width * 0.5f;   // center the child
                pos.y = anchorPosition.y;
                pos.z = anchorPosition.z;

                child.localPosition = pos;

                currentX += width + spacing;
            }
        }
    }




    // calculate anchor postion
    private Vector3 GetAnchorPoint()
    {
        Vector3 extents = boundsSize * 0.5f;
        Vector2 norm = AnchorToNormalized(anchorPosition);

        return new Vector3(
            Mathf.Lerp(-extents.x, extents.x, norm.x),
            Mathf.Lerp(-extents.y, extents.y, norm.y),
            0f
        ) + boundsOffset;

    }
    private static Vector2 AnchorToNormalized(AnchorPosition anchor) => anchor switch
    {
        AnchorPosition.BottomLeft => new Vector2(0f, 0f),
        AnchorPosition.BottomCenter => new Vector2(0.5f, 0f),
        AnchorPosition.BottomRight => new Vector2(1f, 0f),
        AnchorPosition.MiddleLeft => new Vector2(0f, 0.5f),
        AnchorPosition.MiddleCenter => new Vector2(0.5f, 0.5f),
        AnchorPosition.MiddleRight => new Vector2(1f, 0.5f),
        AnchorPosition.TopLeft => new Vector2(0f, 1f),
        AnchorPosition.TopCenter => new Vector2(0.5f, 1f),
        AnchorPosition.TopRight => new Vector2(1f, 1f),
        _ => new Vector2(0.5f, 0.5f),// fallback
    };



    public Vector3 BoundsSize
    {
        get { return boundsSize; }
        set { boundsSize = value; }
    }
    public Vector3 BoundsOffset
    {
        get { return boundsOffset; }
        set { boundsOffset = value; }
    }



    public enum LayoutMode
    {
        Horizontal,
        Vertical
    }
    public enum AnchorPosition
    {
        BottomLeft,
        BottomCenter,
        BottomRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        TopLeft,
        TopCenter,
        TopRight
    }
}
