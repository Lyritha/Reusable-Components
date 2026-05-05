#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

public class ShowOnlyAttribute : PropertyAttribute { }


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif
