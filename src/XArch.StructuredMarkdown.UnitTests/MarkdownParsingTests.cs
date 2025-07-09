using System.Text;

using Snapshooter.Xunit;

using XArch.StructuredMarkdown.Parsing;
using XArch.StructuredMarkdown.Serialization;

namespace XArch.StructuredMarkdown.UnitTests
{
    public class MarkdownParsingTests
    {
        [Fact]
        public void CanParseOnlyFrontmatter()
        {
            string markdown = File.ReadAllText("./test-data/markdown/only_frontmatter.md");

            StructuredMarkdownDocument document = StructuredMarkdownParser.Parse(markdown);
            document.MatchSnapshot();
        }

        [Fact]
        public void CanParseFullDocumentWithNoFrontMatter()
        {
            string markdown = File.ReadAllText("./test-data/markdown/structured_markdown_no_frontmatter.md");

            StructuredMarkdownDocument document = StructuredMarkdownParser.Parse(markdown);
            document.MatchSnapshot();
        }

        [Fact]
        public void CanParseAndSerializeFullDocument2()
        {
            string markdown = File.ReadAllText("./test-data/markdown/structured_markdown_with_frontmatter.md");

            StructuredMarkdownDocument document = StructuredMarkdownParser.Parse(markdown);
            StructuredMarkdownSerializer
                .ToMarkdown(document)
                .MatchSnapshot();
        }

        [Fact]
        public void CanParseAndSerializeFullDocument()
        {
            string markdown = File.ReadAllText("./test-data/markdown/structured_markdown_no_frontmatter.md");

            StructuredMarkdownDocument document = StructuredMarkdownParser.Parse(markdown);
            StructuredMarkdownSerializer
                .ToMarkdown(document)
                .MatchSnapshot();
        }

        [Fact]
        public void CanParseSection()
        {
            string markdown = File.ReadAllText("./test-data/markdown/section.md");

            StructuredMarkdownDocument document = StructuredMarkdownParser.Parse(markdown);
            document.MatchSnapshot();
        }

        [Fact]
        public void CanParseParagraph()
        {
            string markdown = File.ReadAllText("./test-data/markdown/paragraph.md");

            StructuredMarkdownDocument document = StructuredMarkdownParser.Parse(markdown);
            document.MatchSnapshot();
        }
    }
}
