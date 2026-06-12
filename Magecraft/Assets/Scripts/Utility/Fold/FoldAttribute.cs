using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class FoldAttribute : Attribute
{
    public string Name { get; }

    public FoldAttribute() { }

    public FoldAttribute(string name)
    {
        Name = name;
    }
}
