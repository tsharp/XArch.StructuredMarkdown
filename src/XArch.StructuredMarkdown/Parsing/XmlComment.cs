namespace XArch.StructuredMarkdown.Parsing
{
    public class XmlComment
    {
        public bool IsSectionStart { get; set; }
        public bool IsSectionEnd { get; set; }
        public PropertyBag? Metadata { get; set; }
    }
}
