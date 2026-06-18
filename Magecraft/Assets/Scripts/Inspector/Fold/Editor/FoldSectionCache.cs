using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lyrith.Inspector.Fold
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
                    if (TryCreateClassSection(type, "", 0, out FoldSection section))
                        sections.Add(section);
                }
                else
                {
                    FieldInfo[] fields = GetSerializedFields(type).ToArray();
                    bool hasFoldables = fields.Any(f => IsFoldStruct(f) || IsFoldClass(f));

                    if (hasFoldables)
                    {
                        FoldSection mixedSection = new()
                        {
                            ClassName = type.Name,
                            SectionName = type.Name,
                            SectionType = SectionType.Flat
                        };

                        int order = 0;
                        foreach (FieldInfo field in fields) CategorizeField(field, mixedSection, order++);
                        if (mixedSection.Fields.Count > 0 || mixedSection.StructFields.Count > 0 || mixedSection.SubSections.Count > 0)
                            sections.Add(mixedSection);
                    }
                }

                type = type.BaseType;
            }

            sections.Reverse();
            return sections;
        }

        // ─── Section Creation ─────────────────────────────────────────────────────

        private static bool TryCreateClassSection(Type type, string prefix, int order, out FoldSection section)
        {
            section = null;
            if (!Attribute.IsDefined(type, typeof(FoldAttribute))) return false;

            FoldAttribute foldAttr = (FoldAttribute)Attribute.GetCustomAttribute(type, typeof(FoldAttribute));
            section = new FoldSection
            {
                ClassName = type.Name,
                SectionName = !string.IsNullOrEmpty(foldAttr?.Name) ? foldAttr.Name : $"{GetPrettyName(type.Name)} Settings",
                HideFoldout = foldAttr.Hide,
                orderPos = order
            };

            FoldSection currentSubSection = null;
            int innerOrder = 0;

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
                        orderPos = innerOrder++,
                        HideFoldout = startFold.Hide
                    };
                    section.SubSections.Add(currentSubSection);
                }

                FoldSection activeSection = currentSubSection ?? section;
                CategorizeField(field, activeSection, innerOrder++, prefix);

                if (hasEndFold) currentSubSection = null;
            }

            return section.Fields.Count > 0 || section.StructFields.Count > 0 || section.SubSections.Count > 0;
        }

        // ─── Field Categorization ─────────────────────────────────────────────────

        // nested, replacing BuildClassSubSection
        private static void CategorizeField(FieldInfo field, FoldSection section, int order, string prefix = "")
        {
            string path = prefix + field.Name;
            if (IsFoldStruct(field)) section.StructFields.Add((path, order));
            else if (IsFoldClass(field))
            {
                TryCreateClassSection(field.FieldType, path + ".", order, out FoldSection nested);
                section.SubSections.Add(nested);
            }
            else section.Fields.Add((path, order));
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        public static bool IsFoldStruct(FieldInfo field)
        {
            Type t = field.FieldType;
            if (!t.IsValueType || t.IsPrimitive) return false;
            if (!Attribute.IsDefined(t, typeof(SerializableAttribute))) return false;
            return Attribute.IsDefined(t, typeof(FoldAttribute));
        }

        public static bool IsFoldClass(FieldInfo field)
        {
            Type t = field.FieldType;
            if (t.IsValueType) return false;
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