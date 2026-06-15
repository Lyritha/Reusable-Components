using System;

namespace Lyrith.Utility.Fold
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class StartFoldAttribute : Attribute
    {
        public string Name { get; }
        public StartFoldAttribute(string name = null)
        {
            Name = name;
        }
    }
}