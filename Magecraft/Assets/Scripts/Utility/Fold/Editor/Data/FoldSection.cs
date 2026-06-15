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

        public bool IsStructSection = false;
        public bool IsFlat = false;
        public bool Foldout = true;
        public int orderPos = 0;
    }
}
