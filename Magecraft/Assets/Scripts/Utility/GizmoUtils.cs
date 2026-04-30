using UnityEditor;
using UnityEngine;

public static class GizmoUtils
{
    public static void DrawDirectedLine(Vector3 start, Vector3 end, float arrowSize = 0.1f)
    {
        // draw line
        Handles.DrawLine(start, end);

        // draw arrow
        Vector3 arrowPos = Vector3.Lerp(start, end, 0.5f);
        Quaternion rot = Quaternion.LookRotation((end - start).normalized);
        Handles.ConeHandleCap(0, arrowPos, rot, arrowSize, EventType.Repaint);

        // draw points
        Handles.SphereHandleCap(0, start, Quaternion.identity, 0.08f, EventType.Repaint);
        Handles.SphereHandleCap(0, end, Quaternion.identity, 0.08f, EventType.Repaint);
    }
}
