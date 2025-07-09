using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using XArch.StructuredMarkdown.Serialization;

namespace XArch.StructuredMarkdown.Parsing
{
    internal static class StructuredMarkdownParser
    {
        public static StructuredMarkdownDocument Parse(Stream stream)
        {
            PropertyBag? frontmatter = null;
            DocumentSection root = new DocumentSection();

            using (LineCountingStreamReader reader = new LineCountingStreamReader(new StreamReader(stream)))
            {
                string? line = reader.ReadLine(true);
                
                if (line != null && line.Trim() == "---")
                {
                    frontmatter = ParseFrontmatter(reader);
                }

                if (string.IsNullOrWhiteSpace(line)) 
                {
                    return new StructuredMarkdownDocument()
                    {
                        Metadata = frontmatter,
                        Root = root
                    };
                }

                ParseContent(reader, root);

                // finish
                return new StructuredMarkdownDocument()
                {
                    Metadata = frontmatter,
                    Root = root
                };
            }
        }

        private static string? ParseContent(LineCountingStreamReader reader, DocumentSection root, int depth = 0)
        {
            string? line = reader.ReadLine(true);
            bool previousLineWasEmpty = false;
            StringBuilder contentBuilder = new StringBuilder();

            while (line != null)
            {
                // Check for empty lines and skip them if there are multiple in a row
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (previousLineWasEmpty)
                    {
                        line = reader.ReadLine(true);
                        continue;
                    }

                    previousLineWasEmpty = true;
                    contentBuilder.AppendLine();
                    line = reader.ReadLine();
                    continue;
                }

                previousLineWasEmpty = false;

                // Strip any comments from the line content ... for section headers this could be on the preivous line
                // or on the same line
                var xmlCommentResult = line.TryExtractXmlComment(out string? extractedLine, out string? metadata);
                var xmlCommentKind = XmlCommentParser.TryParseMetadata(metadata ?? string.Empty, out PropertyBag? propertyBag);

                if (IsSectionHeader(extractedLine))
                {
                    int headerLevel = 0;
                    // extractedLine is not null here due to IsSectionHeader check
                    while (headerLevel < extractedLine!.Length && extractedLine[headerLevel] == '#')
                        headerLevel++;

                    // If this header is at the same or higher level, return to parent
                    if (headerLevel <= depth)
                        return extractedLine;

                    var newSection = new DocumentSection
                    {
                        Title = extractedLine.TrimStart('#').Trim(),
                        Metadata = xmlCommentKind == XmlCommentKind.SimpleMetadata ? propertyBag : new PropertyBag(),
                    };

                    // Recursively parse the content of the new section
                    string? nextLine = ParseContent(reader, newSection, headerLevel);
                    root.Parts.Add(newSection);

                    // If the next line is a header at the same or higher level, return it to the parent
                    if (nextLine != null && IsSectionHeader(nextLine))
                        return nextLine;

                    line = nextLine ?? reader.ReadLine();
                }
                else if (xmlCommentKind == XmlCommentKind.StartSection)
                {
                    // new section until the returned is a nested 'start section' in which case we need to recurse
                    // or the XmlCommentKind is EndSection
                }
                else
                {
                    contentBuilder.AppendLine(extractedLine);
                    line = reader.ReadLine();
                }
            }

            // If there is any content, add it as a part
            var content = contentBuilder.ToString().TrimEnd();

            if (!string.IsNullOrEmpty(content))
            {
                var xmlCommentResult = content.TryExtractXmlComment(out string? extractedContent, out string? comment);

                if (xmlCommentResult != XmlCommentExtractorResult.Empty)
                {
                }

                var contentSection = new DocumentSection
                {
                    Title = null,
                    Metadata = null
                };

                contentSection.Parts.Add(new DocumentContent(content));
                root.Parts.Add(contentSection);
            }

            return null;
        }

        internal static bool IsSectionHeader(string? line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            return line.StartsWith("#", StringComparison.Ordinal);
        }

        internal static PropertyBag? ParseFrontmatter(LineCountingStreamReader reader)
        {
            StringBuilder builder = new StringBuilder();
            string? line;

            while ((line = reader.ReadLine()) != null && line.Trim() != "---")
            {
                builder.AppendLine(line.TrimEnd());
            }

            return builder.ToString().FromYaml<PropertyBag>();
        }
    }
}
