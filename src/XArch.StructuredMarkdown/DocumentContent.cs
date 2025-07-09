namespace XArch.StructuredMarkdown
{
    public class DocumentContent : DocumentPart
    {
        public DocumentContent()
        {
        }

        public DocumentContent(string content) : this(null, content)
        {
        }

        public DocumentContent(PropertyBag? metadata, string content)
        {
            Content = content;
            Metadata = metadata;
        }

        public string? Content { get; set; }
    }
}
