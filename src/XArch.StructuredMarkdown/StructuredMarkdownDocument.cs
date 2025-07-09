namespace XArch.StructuredMarkdown
{
    using System.IO;
    using System.Text;

    using XArch.StructuredMarkdown.Parsing;

    public class StructuredMarkdownDocument : DocumentPart
    {
        public DocumentSection Root { get; set; } = new DocumentSection();
    }
}
