namespace XArch.StructuredMarkdown
{
    using System.IO;
    using System.Text;

    using XArch.StructuredMarkdown.Parsing;

    public class StructuredMarkdownDocument : DocumentPart
    {
        public DocumentSection Root { get; set; } = new DocumentSection();

        public static StructuredMarkdownDocument Parse(string content) => 
            StructuredMarkdownParser.Parse(new MemoryStream(Encoding.UTF8.GetBytes(content)));

        public static StructuredMarkdownDocument Parse(Stream content) =>
            StructuredMarkdownParser.Parse(content);
    }
}
