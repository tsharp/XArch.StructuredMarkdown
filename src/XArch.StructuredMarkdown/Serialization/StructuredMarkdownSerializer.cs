using System;
using System.Text;

using XArch.StructuredMarkdown.Parsing;

namespace XArch.StructuredMarkdown.Serialization
{
    public static class StructuredMarkdownSerializer
    {
        public static string ToMarkdown(StructuredMarkdownDocument doc)
        {
            var sb = new StringBuilder();

            if (doc.Metadata != null && doc.Metadata.Count > 0)
            {
                sb.AppendLine(doc.Metadata.ToMarkdownFrontmatter());
                sb.AppendLine();
            }

            ToMarkdownSection(doc.Root, sb, 1); // Start with depth 1 for proper heading levels
            return sb.ToString().NormalizeMarkdown();
        }

        private static void ToMarkdownSection(DocumentSection section, StringBuilder sb, int depth)
        {
            foreach (var part in section.Parts)
            {
                if (part is DocumentSection childSection)
                {
                    // Block-level XML section (no title, but has metadata)
                    if (string.IsNullOrEmpty(childSection.Title) && childSection.Metadata != null && childSection.Metadata.Count > 0)
                    {
                        sb.AppendLine(childSection.Metadata.ToXmlComment());
                        ToMarkdownSection(childSection, sb, depth);
                        sb.AppendLine("<!-- /section -->");
                        sb.AppendLine();
                    }
                    // Heading section
                    else if (!string.IsNullOrEmpty(childSection.Title))
                    {
                        var headerLevel = Math.Max(depth, 1);
                        var heading = new string('#', headerLevel) + " " + childSection.Title;
                        if (childSection.Metadata != null && childSection.Metadata.Count > 0)
                        {
                            heading += " " + childSection.Metadata.ToXmlComment();
                        }
                        sb.AppendLine(heading);
                        ToMarkdownSection(childSection, sb, depth + 1);
                    }
                }
                else if (part is DocumentContent content)
                {
                    // Standalone comment for metadata if present
                    if (content.Metadata != null && content.Metadata.Count > 0)
                    {
                        sb.AppendLine(content.Metadata.ToXmlComment());
                    }
                    sb.AppendLine(content.Content?.Trim() ?? string.Empty);
                    sb.AppendLine();
                }
            }
        }
    }

}
