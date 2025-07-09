using System.Text;

namespace XArch.StructuredMarkdown.Serialization
{
    public static class StructuredMarkdownSerializer
    {
        public static string ToMarkdown(StructuredMarkdownDocument doc)
        {
            var sb = new StringBuilder();
            ToMarkdownSection(doc.Root, sb, 0);
            return sb.ToString().Trim();
        }

        private static void ToMarkdownSection(DocumentSection section, StringBuilder sb, int depth)
        {
            foreach (var part in section.Parts)
            {
                if (part is DocumentSection childSection)
                {
                    // If it's a heading section
                    if (!string.IsNullOrEmpty(childSection.Title))
                    {
                        // Output heading and inline metadata if present
                        var headerLevel = depth + 1;
                        var header = new string('#', headerLevel) + " " + childSection.Title;
                        if (childSection.Metadata != null && childSection.Metadata.Count > 0)
                        {
                            header += " " + childSection.Metadata.ToYaml().ToHtmlComment();
                        }
                        sb.AppendLine(header);

                        ToMarkdownSection(childSection, sb, headerLevel);
                    }
                    // Otherwise it's a block-level XML section
                    else if (childSection.Metadata != null && childSection.Metadata.Count > 0)
                    {
                        // Output section open tag
                        sb.AppendLine(childSection.Metadata.ToYaml().ToSectionOpenComment());

                        ToMarkdownSection(childSection, sb, depth);

                        // Output section close tag
                        sb.AppendLine("<!-- /section -->");
                    }
                    // Empty section node (shouldn't occur, but skip)
                }
                else if (part is DocumentContent content)
                {
                    // Output metadata (if present) as standalone comment
                    if (content.Metadata != null && content.Metadata.Count > 0)
                    {
                        sb.AppendLine(content.Metadata.ToYaml().ToHtmlComment());
                    }
                    // Output the content itself
                    sb.AppendLine(content.Content.Trim());
                    sb.AppendLine();
                }
            }
        }

        // Helper for section open tag: comma-separated for spec
        private static string ToSectionOpenComment(this string yaml)
        {
            // Example: "section: foo, tags: [bar]" → <!-- section: foo, tags: [bar] -->
            // Remove trailing newline, brackets, etc.
            yaml = yaml.Trim();
            if (yaml.StartsWith("---")) yaml = yaml.Substring(3).Trim();
            if (yaml.EndsWith("---")) yaml = yaml.Substring(0, yaml.Length - 3).Trim();
            // Remove the comment brackets if any
            if (yaml.StartsWith("<!--")) yaml = yaml.Substring(4).Trim();
            if (yaml.EndsWith("-->")) yaml = yaml.Substring(0, yaml.Length - 3).Trim();
            return $"<!-- section: {yaml} -->";
        }
    }

}
