using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(TransformLayout))]
public class TransformLayoutEditor : Editor
{
    private BoxBoundsHandle _boundsHandle = new BoxBoundsHandle();

    private void OnSceneGUI()
    {
        TransformLayout layout = (TransformLayout)target;

        // Determine correct matrix (prefab mode vs normal scene)
        Matrix4x4 matrix = GetCorrectMatrix(layout);

        using (new Handles.DrawingScope(matrix))
        {
            _boundsHandle.center = layout.BoundsOffset;
            _boundsHandle.size = layout.BoundsSize;

            Undo.RecordObject(layout, "Modify Layout Bounds");

            EditorGUI.BeginChangeCheck();
            _boundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(layout, "Modify Layout Bounds");

                layout.BoundsOffset = _boundsHandle.center;
                layout.BoundsSize = _boundsHandle.size;

                layout.ApplyLayout();
                EditorUtility.SetDirty(layout);
            }
        }
    }

    private Matrix4x4 GetCorrectMatrix(TransformLayout layout)
    {
        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

        if (stage != null)
        {
            // In prefab mode: use the prefab root transform
            return stage.prefabContentsRoot.transform.localToWorldMatrix;
        }

        // Normal scene mode
        return layout.transform.localToWorldMatrix;
    }
}
