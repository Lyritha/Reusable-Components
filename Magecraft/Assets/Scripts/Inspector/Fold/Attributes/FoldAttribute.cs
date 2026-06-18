using System;

namespace Lyrith.Inspector.Fold
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class FoldAttribute : Attribute
    {
        public string Name { get; }
        public bool Hide { get; }

        public FoldAttribute() { }
        public FoldAttribute(string name = null, bool hide = false)
        {
            Name = name;
            Hide = hide;
        }
    }
}