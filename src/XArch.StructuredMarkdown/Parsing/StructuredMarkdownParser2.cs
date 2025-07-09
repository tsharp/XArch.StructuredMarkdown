using System;
using System.IO;
using System.Text;

namespace XArch.StructuredMarkdown.Parsing
{
    public static class StructuredMarkdownParser2
    {
        public static string NormalizeMarkdown(this string? markdown)
        {
            var cleanedMarkdown = markdown?.Trim() ?? string.Empty;
            cleanedMarkdown = cleanedMarkdown.Replace("\r\n", "\n").Replace("\r", "\n");
            return cleanedMarkdown.Trim();
        }

        public static StructuredMarkdownDocument Parse(string markdown)
        {
            
            return Parse(new MemoryStream(Encoding.UTF8.GetBytes(markdown.NormalizeMarkdown())));
        }

        private static StructuredMarkdownDocument Parse(Stream stream)
        {
            PropertyBag? frontmatter = null;
            var root = new DocumentSection();

            using (var reader = new LineCountingStreamReader(new StreamReader(stream)))
            {
                string? line = reader.ReadLine(true);

                if (line != null && line.Trim() == "---")
                {
                    frontmatter = StructuredMarkdownParser.ParseFrontmatter(reader);
                    line = reader.ReadLine(true); // Advance past frontmatter
                }

                ParseContent(reader, root, ref line, 0);

                return new StructuredMarkdownDocument()
                {
                    Metadata = frontmatter,
                    Root = root
                };
            }
        }

        // === MAIN DISPATCH ===
        private static void ParseContent(LineCountingStreamReader reader, DocumentSection root, ref string? line, int depth)
        {
            PropertyBag? pendingMetadata = null;

            while (line != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    line = reader.ReadLine(true);
                    continue;
                }

                var xmlCommentResult = line.TryExtractXmlComment(out string? extractedLine, out string? metadata);
                var xmlCommentKind = XmlCommentParser.TryParseMetadata(metadata ?? string.Empty, out PropertyBag? propertyBag);

                if (xmlCommentKind == XmlCommentKind.StartSection && string.IsNullOrWhiteSpace(extractedLine))
                {
                    line = ParseXmlSection(reader, root, propertyBag, ref line, depth);
                    continue;
                }

                if (xmlCommentKind == XmlCommentKind.EndSection)
                {
                    line = reader.ReadLine(true);
                    continue;
                }

                if (xmlCommentKind == XmlCommentKind.SimpleMetadata && string.IsNullOrWhiteSpace(extractedLine))
                {
                    pendingMetadata = propertyBag;
                    line = reader.ReadLine(true);
                    continue;
                }

                if (IsSectionHeader(extractedLine))
                {
                    line = ParseMarkdownSection(reader, root, extractedLine!, metadata, pendingMetadata, ref line, depth);
                    pendingMetadata = null;
                    continue;
                }

                // Always add content directly as DocumentContent
                line = ParseParagraph(reader, root, extractedLine, pendingMetadata, ref line);
                pendingMetadata = null;
            }
        }

        // === PARAGRAPH PARSER ===
        private static string? ParseParagraph(LineCountingStreamReader reader, DocumentSection root, string? firstLine, PropertyBag? metadata, ref string? line)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(firstLine))
                builder.AppendLine(firstLine);

            while (true)
            {
                var nextLine = reader.ReadLine(true);
                if (nextLine == null) { line = null; break; }
                if (string.IsNullOrWhiteSpace(nextLine)) { line = reader.ReadLine(true); break; }

                var xmlCommentResult = nextLine.TryExtractXmlComment(out string? paraContent, out string? _);
                if (xmlCommentResult == XmlCommentExtractorResult.Comment && string.IsNullOrWhiteSpace(paraContent))
                {
                    line = nextLine;
                    break;
                }
                if (!string.IsNullOrWhiteSpace(paraContent))
                    builder.AppendLine(paraContent);
            }

            var text = builder.ToString().TrimEnd();
            if (!string.IsNullOrEmpty(text))
            {
                var docContent = new DocumentContent(text) { Metadata = metadata };
                root.Parts.Add(docContent);
            }
            return line;
        }

        // === MARKDOWN HEADING/SECTION PARSER ===
        private static string? ParseMarkdownSection(
            LineCountingStreamReader reader,
            DocumentSection root,
            string headingLine,
            string? headingMetadata,
            PropertyBag? pendingMetadata,
            ref string? line,
            int parentDepth)
        {
            int headerLevel = 0;
            while (headerLevel < headingLine.Length && headingLine[headerLevel] == '#')
                headerLevel++;
            string title = headingLine.TrimStart('#').Trim();

            PropertyBag? sectionMetadata = null;
            if (!string.IsNullOrWhiteSpace(headingMetadata) && XmlCommentParser.TryParseMetadata(headingMetadata, out PropertyBag? meta) == XmlCommentKind.SimpleMetadata)
            {
                sectionMetadata = meta;
            }
            else if (pendingMetadata != null)
            {
                sectionMetadata = pendingMetadata;
            }

            var section = new DocumentSection { Title = title, Metadata = sectionMetadata };
            root.Parts.Add(section);

            string? nextLine = reader.ReadLine(true);

            while (nextLine != null)
            {
                var peekResult = nextLine.TryExtractXmlComment(out string? peekLine, out string? _);
                if (IsSectionHeader(peekLine))
                {
                    int nextHeaderLevel = 0;
                    while (nextHeaderLevel < peekLine!.Length && peekLine[nextHeaderLevel] == '#')
                        nextHeaderLevel++;
                    if (nextHeaderLevel <= headerLevel)
                    {
                        line = nextLine;
                        return line;
                    }
                }

                ParseContent(reader, section, ref nextLine, headerLevel);
                if (nextLine == null) break;
            }
            line = nextLine;
            return line;
        }

        // === XML COMMENT SECTION PARSER (BLOCK-LEVEL) ===
        private static string? ParseXmlSection(
    LineCountingStreamReader reader,
    DocumentSection root,
    PropertyBag? sectionMetadata,
    ref string? line,
    int depth)
        {
            var section = new DocumentSection { Title = null, Metadata = sectionMetadata };
            root.Parts.Add(section);

            string? nextLine = reader.ReadLine(true);

            while (nextLine != null)
            {
                var xmlCommentResult = nextLine.TryExtractXmlComment(out string? extractedLine, out string? metadata);
                var xmlCommentKind = XmlCommentParser.TryParseMetadata(metadata ?? string.Empty, out PropertyBag? childSectionBag);

                // Handle nested section: recurse!
                if (xmlCommentKind == XmlCommentKind.StartSection && string.IsNullOrWhiteSpace(extractedLine))
                {
                    nextLine = ParseXmlSection(reader, section, childSectionBag, ref nextLine, depth);
                    continue;
                }

                // Handle section end: close only this section and return to caller
                if (xmlCommentKind == XmlCommentKind.EndSection)
                {
                    line = reader.ReadLine(true);
                    return line;
                }

                // Handle headings inside section
                if (IsSectionHeader(extractedLine))
                {
                    nextLine = ParseMarkdownSection(reader, section, extractedLine!, metadata, null, ref nextLine, depth);
                    continue;
                }

                // If line has content or metadata, treat as paragraph
                if (!string.IsNullOrWhiteSpace(extractedLine) || xmlCommentKind == XmlCommentKind.SimpleMetadata)
                {
                    nextLine = ParseParagraph(reader, section, extractedLine, null, ref nextLine);
                    continue;
                }

                // Otherwise, keep parsing (could be blank line, etc.)
                nextLine = reader.ReadLine(true);
            }

            line = nextLine;
            return line;
        }

        private static bool IsSectionHeader(string? line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;
            int i = 0;
            while (i < line.Length && line[i] == '#') i++;
            if (i == 0) return false;
            return i < line.Length ? char.IsWhiteSpace(line[i]) : true;
        }
    }
}
