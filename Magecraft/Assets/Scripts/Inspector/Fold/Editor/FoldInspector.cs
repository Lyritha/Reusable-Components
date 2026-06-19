#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lyrith.Inspector.Fold
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class FoldInspector : Editor
    {
        private Dictionary<string, SerializedProperty> propertyCache;
        private List<FoldSection> foldSections;
        private GUIStyle boxStyle;
        private GUIStyle labelStyle;

        private void OnEnable()
        {
            foldSections = FoldSectionCache.Build(target);
            propertyCache = new();
        }

        public override void OnInspectorGUI()
        {
            InitStyles();
            serializedObject.Update();

            // draw script field
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(GetProp("m_Script"));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(6);

            foreach (FoldSection section in foldSections) DrawFoldSection(section);

            DrawRemainingProperties();
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawFoldSection(FoldSection section)
        {
            if (section.HideFoldout) return;

            switch (section.SectionType)
            {
                case SectionType.Flat:
                    foreach (FoldOrderedItem item in GetOrderedItems(section)) DrawOrderedItem(item, true);
                    break;

                case SectionType.Struct:
                    foreach ((string name, int _) in section.StructFields) DrawStructBox(name, true);
                    EditorGUILayout.Space(6);
                    break;

                case SectionType.Default:
                default:
                    DrawFoldoutBox(section, () =>
                    { foreach (FoldOrderedItem item in GetOrderedItems(section)) DrawOrderedItem(item); });
                    EditorGUILayout.Space(6);
                    break;
            }
        }


        private void DrawOrderedItem(FoldOrderedItem item, bool indent = false)
        {
            if (item.Section != null && item.Section.HideFoldout) return;

            switch (item.Type)
            {
                case FoldItemType.Field: DrawProperty(item.Name); break;

                case FoldItemType.Struct: DrawStructBox(item.Name, indent); break;

                case FoldItemType.SubSection:
                    DrawFoldoutBox(item.Section, () =>
                    {
                        foreach (FoldOrderedItem subItem in GetOrderedItems(item.Section)) DrawOrderedItem(subItem);
                    });
                    break;
            }
        }
        private void DrawProperty(string name)
        {
            SerializedProperty prop = GetProp(name);
            if (prop == null) return;

            EditorGUILayout.PropertyField(prop, true);
        }
        private void DrawStructBox(string name, bool indent = false)
        {
            SerializedProperty prop = GetProp(name);
            if (prop == null) return;

            EditorGUILayout.Space(3);

            DrawBox(() =>
            {
                EditorGUILayout.PropertyField(prop, true);
            }, indent);

            EditorGUILayout.Space(3);
        }
        private void DrawRemainingProperties()
        {
            HashSet<string> excluded = new() { "m_Script" };
            foreach (FoldSection s in foldSections) CollectExcluded(s, excluded);
            DrawPropertiesExcluding(serializedObject, excluded.ToArray());
        }

        private static void CollectExcluded(FoldSection section, HashSet<string> excluded)
        {
            foreach ((string name, int _) in section.Fields) excluded.Add(name.Split('.')[0]);
            foreach ((string name, int _) in section.StructFields) excluded.Add(name.Split('.')[0]);
            foreach (FoldSection sub in section.SubSections)
            {
                excluded.Add(sub.ClassName);
                CollectExcluded(sub, excluded);
            }
        }


        // ui utils
        private void DrawFoldoutBox(FoldSection section, Action contents)
        {
            DrawBox(() =>
            {
                section.Foldout = EditorGUILayout.Foldout(section.Foldout, section.SectionName, true, labelStyle);
                if (section.Foldout)
                {
                    EditorGUILayout.Space(6);
                    contents();
                }
            });
        }
        private void DrawBox(Action contents, bool indent = true)
        {
            try
            {
                EditorGUILayout.BeginVertical(boxStyle);
                if (indent) EditorGUI.indentLevel++;
                contents?.Invoke();
            }
            finally
            {
                if (indent) EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }
        private void InitStyles()
        {
            boxStyle ??= new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10) };
            labelStyle ??= new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
        }


        // utils
        private SerializedProperty GetProp(string name)
        {
            if (propertyCache.TryGetValue(name, out var prop) && prop != null) return prop;

            prop = serializedObject.FindProperty(name);
            if (prop != null) propertyCache[name] = prop;

            return prop;
        }
        private static IEnumerable<FoldOrderedItem> GetOrderedItems(FoldSection section)
        {
            IEnumerable<FoldOrderedItem> fields = section.Fields.Select(f => new FoldOrderedItem
            { Order = f.Item2, Type = FoldItemType.Field, Name = f.Item1 });

            IEnumerable<FoldOrderedItem> structs = section.StructFields.Select(f => new FoldOrderedItem
            { Order = f.Item2, Type = FoldItemType.Struct, Name = f.Item1 });

            IEnumerable<FoldOrderedItem> subs = section.SubSections.Select(s => new FoldOrderedItem
            { Order = s.orderPos, Type = FoldItemType.SubSection, Section = s });

            return fields.Concat(structs).Concat(subs).OrderBy(x => x.Order);
        }
    }
}
#endif