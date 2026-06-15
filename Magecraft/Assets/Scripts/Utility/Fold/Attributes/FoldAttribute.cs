using System;

namespace Lyrith.Utility.Fold
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class FoldAttribute : Attribute
    {
        public string Name { get; }
        public FoldAttribute() { }
        public FoldAttribute(string name) { Name = name; }
    }
}