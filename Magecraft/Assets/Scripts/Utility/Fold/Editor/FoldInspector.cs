using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class FoldInspector : Editor
{
    private List<FoldSection> foldSections;
    private GUIStyle boxStyle;
    private GUIStyle headerStyle;

    private void Style()
    {
        boxStyle ??= new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10) };
        headerStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
    }

    private void OnEnable() => CacheFoldSections();

    private void CacheFoldSections()
    {
        foldSections = new();
        Type type = target.GetType();

        while (type != typeof(MonoBehaviour) && type != null)
        {
            if (Attribute.IsDefined(type, typeof(FoldAttribute)))
            {
                var foldAttr = (FoldAttribute)Attribute.GetCustomAttribute(type, typeof(FoldAttribute));

                FoldSection section = new()
                {
                    ClassName = type.Name,
                    SectionName = !string.IsNullOrEmpty(foldAttr?.Name) ? foldAttr.Name : $"{GetPrettyName(type.Name)} Settings"
                };


                FieldInfo[] fields = type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );

                foreach (FieldInfo field in fields)
                {
                    bool isPublic = field.IsPublic;
                    bool isSerialized = field.IsDefined(typeof(SerializeField), true);

                    if (isPublic || isSerialized) section.Fields.Add(field.Name);
                }

                // Skip empty foldouts
                if (section.Fields.Count > 0) foldSections.Add(section);
            }

            type = type.BaseType;
        }

        // re-orders the inspector properly
        foldSections.Reverse();
    }



    public override void OnInspectorGUI()
    {
        Style();
        serializedObject.Update();

        // Script field
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(6);

        // Draw foldouts for each folded class
        foreach (FoldSection section in foldSections)
        {
            section.Foldout = EditorGUILayout.BeginFoldoutHeaderGroup(section.Foldout, section.SectionName);

            if (section.Foldout)
            {
                EditorGUILayout.BeginVertical(boxStyle);
                foreach (string field in section.Fields) DrawProperty(field);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(6);
        }

        // Exclude all folded fields + script
        List<string> excluded = new() { "m_Script" };
        excluded.AddRange(foldSections.SelectMany(s => s.Fields));

        DrawPropertiesExcluding(serializedObject, excluded.ToArray());
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawProperty(string name)
    {
        SerializedProperty prop = serializedObject.FindProperty(name);
        if (prop != null) EditorGUILayout.PropertyField(prop);
    }

    private string GetPrettyName(string raw)
    {
        int index = raw.IndexOf('`');
        if (index >= 0) raw = raw[..index];

        return raw;
    }



    private class FoldSection
    {
        public string ClassName;
        public string SectionName;

        public List<string> Fields = new();
        public bool Foldout = true;
    }
}
