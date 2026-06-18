using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lyrith.Inspector.DynamicDropdown
{
    [CustomPropertyDrawer(typeof(DynamicDropdownAttribute))]
    public class DynamicDropdownDrawer : PropertyDrawer
    {
        private struct DropdownData
        {
            public object[] values;
            public string[] labels;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DynamicDropdownAttribute attribute = (DynamicDropdownAttribute)base.attribute;

            object declaringObject = GetDeclaringObject(property);
            if (declaringObject == null)
            {
                EditorGUI.LabelField(position, label.text, "Could not resolve declaring object");
                return;
            }

            MethodInfo method = declaringObject.GetType().GetMethod(attribute.MethodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null)
            {
                EditorGUI.LabelField(position, label.text, $"Method '{attribute.MethodName}' not found");
                return;
            }

            object result = method.Invoke(method.IsStatic ? null : declaringObject, null);
            if (!TryConvertToDropdownData(result, out DropdownData dropdown))
            {
                EditorGUI.LabelField(position, label.text, "method result is not of (T, string)");
                return;
            }

            if (dropdown.labels.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "<empty>");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            int currentIndex = GetCurrentIndex(property, dropdown.values);
            int selected = EditorGUI.Popup(position, currentIndex, dropdown.labels);
            if (selected >= 0 && selected < dropdown.values.Length)
            {
                SetSerializedValue(property, dropdown.values[selected]);
            }
            EditorGUI.EndProperty();
        }
        private static bool TryConvertToDropdownData(object value, out DropdownData dropdown)
        {
            dropdown = new DropdownData { values = Array.Empty<object>(), labels = Array.Empty<string>() };
            if (value is not IEnumerable enumerable) return false;

            object[] items = enumerable.Cast<object>().ToArray();

            // Empty collection is valid (renders as "<empty>"), nothing to type-check.
            if (items.Length == 0) return true;

            // Every item must be a 2-field ValueTuple<T, string>.
            object[] values = new object[items.Length];
            string[] labels = new string[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                if (!TryExtractTuple(items[i], out object v, out string lbl)) return false;
                values[i] = v;
                labels[i] = lbl;
            }

            dropdown = new DropdownData { values = values, labels = labels };
            return true;
        }
        private static bool TryExtractTuple(object item, out object value, out string label)
        {
            value = null;
            label = null;

            if (item == null) return false;

            // if type is not a tuple, return false
            Type type = item.GetType();
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ValueTuple<,>)) return false;

            // is second element isn't string, return false
            Type[] genericArgs = type.GetGenericArguments();
            if (genericArgs[1] != typeof(string)) return false;

            // get the final result as the correct type
            FieldInfo[] fields = type.GetFields();
            value = fields[0].GetValue(item);
            label = (string)fields[1].GetValue(item) ?? "<null>";
            return true;
        }

        private static int GetCurrentIndex(SerializedProperty property, object[] values)
        {
            if (values == null || values.Length == 0) return 0;

            Func<object, bool> matches = property.propertyType switch
            {
                SerializedPropertyType.String => v => v?.ToString() == property.stringValue,
                SerializedPropertyType.Integer => v => v is int i && i == property.intValue,
                SerializedPropertyType.Float => v => v is float f && Mathf.Approximately(f, property.floatValue),
                SerializedPropertyType.ObjectReference => v => Equals(v, property.objectReferenceValue),
                SerializedPropertyType.ManagedReference => v => Equals(v, property.managedReferenceValue),
                SerializedPropertyType.Generic => v => Equals(v, GetValueViaReflection(property)),
                _ => v => false
            };

            int index = Array.FindIndex(values, v => matches(v));
            return index >= 0 ? index : 0;
        }

        private static object GetValueViaReflection(SerializedProperty property)
        {
            object declaringObject = GetDeclaringObject(property);
            if (declaringObject == null) return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            string lastElement = path.Split('.')[^1];

            if (lastElement.Contains("["))
            {
                string fieldName = lastElement[..lastElement.IndexOf('[')];
                int index = int.Parse(lastElement[(lastElement.IndexOf('[') + 1)..].TrimEnd(']'));
                return GetFieldValue(declaringObject, fieldName) is IEnumerable enumerable
                    ? enumerable.Cast<object>().ElementAtOrDefault(index)
                    : null;
            }

            return GetFieldValue(declaringObject, lastElement);
        }

        private static object GetFieldValue(object source, string fieldName)
        {
            if (source == null) return null;

            Type type = source.GetType();
            FieldInfo field = null;

            while (field == null && type != null)
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            }

            return field?.GetValue(source);
        }

        private static void SetSerializedValue(SerializedProperty property, object value)
        {
            if (value == null)
                return;
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = value.ToString();
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = Convert.ToInt32(value);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = Convert.ToSingle(value);
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = value as UnityEngine.Object;
                    break;
                default:
                    property.managedReferenceValue = value;
                    break;
            }
        }

        /// <summary>
        /// Resolves the object instance that actually declares the field this property points to,
        /// by walking the property path up to (but not including) the final segment.
        /// </summary>
        private static object GetDeclaringObject(SerializedProperty property)
        {
            object target = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');

            // Stop one element early — we want the *container* of the final field, not the field itself.
            for (int i = 0; i < elements.Length - 1; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string fieldName = element[..element.IndexOf('[')];
                    int index = int.Parse(element[(element.IndexOf('[') + 1)..].TrimEnd(']'));
                    target = GetFieldValue(target, fieldName) is IEnumerable enumerable
                        ? enumerable.Cast<object>().ElementAtOrDefault(index)
                        : null;
                }
                else
                {
                    target = GetFieldValue(target, element);
                }

                if (target == null) return null;
            }

            return target;
        }
    }
}