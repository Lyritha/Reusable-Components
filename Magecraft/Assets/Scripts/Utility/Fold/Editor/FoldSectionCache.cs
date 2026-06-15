using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lyrith.Utility.Fold.EditorTools
{
    public static class FoldSectionCache
    {
        public static List<FoldSection> Build(Object target)
        {
            List<FoldSection> sections = new();
            Type type = target.GetType();

            while (type != typeof(MonoBehaviour) && type != null)
            {
                if (Attribute.IsDefined(type, typeof(FoldAttribute)))
                {
                    if (TryCreateClassSection(type, out FoldSection section))
                        sections.Add(section);
                }
                else
                {
                    FieldInfo[] fields = GetSerializedFields(type).ToArray();
                    bool hasFoldStructs = fields.Any(IsFoldStruct);

                    if (hasFoldStructs)
                    {
                        FoldSection mixedSection = new()
                        {
                            ClassName = type.Name,
                            SectionName = type.Name,
                            IsFlat = true
                        };

                        int order = 0;
                        foreach (FieldInfo field in fields) CategorizeField(field, mixedSection, order++);
                        if (mixedSection.Fields.Count > 0 || mixedSection.StructFields.Count > 0) sections.Add(mixedSection);
                    }
                }

                type = type.BaseType;
            }

            sections.Reverse();
            return sections;
        }

        // ─── Section Creation ─────────────────────────────────────────────────────

        private static bool TryCreateClassSection(Type type, out FoldSection section)
        {
            section = null;
            if (!Attribute.IsDefined(type, typeof(FoldAttribute))) return false;

            FoldAttribute foldAttr = (FoldAttribute)Attribute.GetCustomAttribute(type, typeof(FoldAttribute));
            section = new FoldSection
            {
                ClassName = type.Name,
                SectionName = !string.IsNullOrEmpty(foldAttr?.Name) ? foldAttr.Name : $"{GetPrettyName(type.Name)} Settings"
            };

            FoldSection currentSubSection = null;
            int order = 0;

            foreach (FieldInfo field in GetSerializedFields(type))
            {
                StartFoldAttribute startFold = field.GetCustomAttribute<StartFoldAttribute>();
                bool hasEndFold = field.IsDefined(typeof(EndFoldAttribute), false);

                if (startFold != null)
                {
                    currentSubSection = new FoldSection
                    {
                        ClassName = field.Name,
                        SectionName = !string.IsNullOrEmpty(startFold.Name) ? startFold.Name : ObjectNames.NicifyVariableName(field.Name),
                        orderPos = order++
                    };
                    section.SubSections.Add(currentSubSection);
                }

                FoldSection activeSection = currentSubSection ?? section;
                CategorizeField(field, activeSection, order++);

                if (hasEndFold) currentSubSection = null;
            }

            return section.Fields.Count > 0 || section.StructFields.Count > 0 || section.SubSections.Count > 0;
        }

        private static FoldSection CreateStructSection(FieldInfo field, int order)
        {
            FoldAttribute foldAttr = (FoldAttribute)Attribute.GetCustomAttribute(field.FieldType, typeof(FoldAttribute));
            return new FoldSection
            {
                ClassName = field.FieldType.Name,
                SectionName = !string.IsNullOrEmpty(foldAttr?.Name) ? foldAttr.Name : ObjectNames.NicifyVariableName(field.Name),
                StructFields = { (field.Name, order) },
                IsStructSection = true,
                orderPos = order
            };
        }

        // ─── Field Categorization ─────────────────────────────────────────────────

        private static void CategorizeField(FieldInfo field, FoldSection section, int order)
        {
            if (IsFoldStruct(field)) section.StructFields.Add((field.Name, order));
            else section.Fields.Add((field.Name, order));
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        public static bool IsFoldStruct(FieldInfo field)
        {
            Type t = field.FieldType;
            if (!t.IsValueType || t.IsPrimitive) return false;
            if (!Attribute.IsDefined(t, typeof(SerializableAttribute))) return false;
            return Attribute.IsDefined(t, typeof(FoldAttribute));
        }

        public static IEnumerable<FieldInfo> GetSerializedFields(Type type) => type
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .Where(f => f.IsPublic || f.IsDefined(typeof(SerializeField), true));

        public static string GetPrettyName(string raw)
        {
            int index = raw.IndexOf('`');
            return index >= 0 ? raw[..index] : raw;
        }
    }
}