using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lyrith.Utility.Fold.EditorTools
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class FoldInspector : Editor
    {
        // ─── State ───────────────────────────────────────────────────────────────

        private List<FoldSection> foldSections;
        private GUIStyle boxStyle;
        private GUIStyle labelStyle;

        // ─── Unity Callbacks ─────────────────────────────────────────────────────

        private void OnEnable() => foldSections = FoldSectionCache.Build(target);

        public override void OnInspectorGUI()
        {
            InitStyles();
            serializedObject.Update();

            DrawScriptField();
            EditorGUILayout.Space(6);

            foreach (FoldSection section in foldSections) DrawFoldSection(section);

            DrawRemainingProperties();
            serializedObject.ApplyModifiedProperties();
        }

        // ─── Section Drawing ──────────────────────────────────────────────────────

        private void DrawFoldSection(FoldSection section)
        {
            if (section.IsFlat)
            {
                foreach (FoldOrderedItem item in GetOrderedItems(section)) DrawOrderedItem(item);
                return;
            }

            if (section.IsStructSection)
            {
                foreach ((string name, int _) in section.StructFields) DrawStructBox(name, true);
                EditorGUILayout.Space(6);
                return;
            }

            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUI.indentLevel++;
            section.Foldout = EditorGUILayout.Foldout(section.Foldout, section.SectionName, true, labelStyle);

            if (section.Foldout)
            {
                EditorGUILayout.Space(6);

                foreach (FoldOrderedItem item in GetOrderedItems(section))
                    DrawOrderedItem(item);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(6);
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

        private void DrawOrderedItem(FoldOrderedItem item)
        {
            switch (item.Type)
            {
                case FoldItemType.Field: DrawProperty(item.Name); break;
                case FoldItemType.Struct: DrawStructBox(item.Name); break;
                case FoldItemType.SubSection: DrawSubSection(item.Section); break;
            }
        }

        private void DrawScriptField()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();
        }
        private void DrawProperty(string name)
        {
            SerializedProperty prop = serializedObject.FindProperty(name);
            if (prop == null) return;

            if (prop.propertyType == SerializedPropertyType.Generic && prop.hasVisibleChildren)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(prop, true);
                EditorGUI.indentLevel--;
            }
            else EditorGUILayout.PropertyField(prop);
        }
        private void DrawStructBox(string name, bool indent = false)
        {
            SerializedProperty prop = serializedObject.FindProperty(name);
            if (prop == null) return;

            EditorGUILayout.Space(3);

            EditorGUILayout.BeginVertical(boxStyle);
            if (indent) EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(prop, true);
            if (indent) EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(3);
        }

        private void DrawSubSection(FoldSection section)
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUI.indentLevel++;
            section.Foldout = EditorGUILayout.Foldout(section.Foldout, section.SectionName, true, labelStyle);

            if (section.Foldout) foreach (FoldOrderedItem item in GetOrderedItems(section)) DrawOrderedItem(item);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void DrawRemainingProperties()
        {
            List<string> excluded = new() { "m_Script" };
            excluded.AddRange(foldSections.SelectMany(s => s.Fields.Select(f => f.Item1)));
            excluded.AddRange(foldSections.SelectMany(s => s.StructFields.Select(f => f.Item1)));
            excluded.AddRange(foldSections.SelectMany(s => s.SubSections.SelectMany(sub => sub.Fields.Select(f => f.Item1))));
            excluded.AddRange(foldSections.SelectMany(s => s.SubSections.SelectMany(sub => sub.StructFields.Select(f => f.Item1))));
            DrawPropertiesExcluding(serializedObject, excluded.ToArray());
        }
        private void InitStyles()
        {
            boxStyle ??= new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(10, 10, 10, 10) };
            labelStyle ??= new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };
        }
    }
}