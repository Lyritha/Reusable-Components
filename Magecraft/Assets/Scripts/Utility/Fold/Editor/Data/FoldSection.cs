using System.Collections.Generic;

namespace Lyrith.Utility.Fold.EditorTools
{
    public class FoldSection
    {
        public string ClassName;
        public string SectionName;

        public List<(string, int)> Fields = new();
        public List<(string, int)> StructFields = new();

        public List<FoldSection> SubSections = new();
        public SectionType SectionType = SectionType.Default;

        public bool Foldout = true;
        public bool HideFoldout = false;
        public int orderPos = 0;
    }
}
