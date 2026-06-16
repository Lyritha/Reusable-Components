using System;

namespace Lyrith.Utility.Fold
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class StartFoldAttribute : Attribute
    {
        public string Name { get; }
        public bool Hide { get; }

        public StartFoldAttribute(string name = null, bool hide = false)
        {
            Name = name;
            Hide = hide;
        }
    }
}