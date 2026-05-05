using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class BulletBehaviour : ScriptableObject
{
    public string BehaviorName;
    [TextArea]
    public string BehaviorInfo;

    public BulletBehaviorType Type;

    public abstract void Execute(GameObject bullet);

    public List<FieldData> GetValues()
    {
        List<FieldData > result = new();

        // BindingFlags to get public + serialized fields
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // Only include serialized fields
            if (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))
            {
                FieldData data = new()
                {
                    FieldName = field.Name,
                    FieldType = field.FieldType,
                    Value = field.GetValue(this)
                };

                result.Add(data);
            }
        }

        return result;
    }
}

public struct FieldData { 
    public string FieldName;
    public Type FieldType;
    public object Value;
}

public enum BulletBehaviorType
{
    Start,
    Middle,
    End
}