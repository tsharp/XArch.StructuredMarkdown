using System.Collections.Generic;

namespace XArch.StructuredMarkdown
{
    public class DocumentSection : DocumentPart
    {
        public string? Title { get; set; }

        public HashSet<DocumentPart> Parts { get; set; } = [];
    }
}
