namespace XArch.StructuredMarkdown.UnitTests
{
    using XArch.StructuredMarkdown.Parsing;

    public class CommentExtractionTests
    {
        [Fact]
        public void CanExtractSingleLineHtmlComment()
        {
            string input = "<!-- This is a comment --> Some content";
            XmlCommentExtractorResult result = input.TryExtractXmlComment(out string? output, out string? comment);
            Assert.Equal(XmlCommentExtractorResult.Comment, result);
            Assert.Equal("Some content", output);
            Assert.Equal("This is a comment", comment);
        }

        [Fact]
        public void CanExtractHtmlCommentFromMarkdownHeader()
        {
            string input = "# Title <!-- This is a comment -->";
            XmlCommentExtractorResult result = input.TryExtractXmlComment(out string? output, out string? comment);
            Assert.Equal(XmlCommentExtractorResult.Comment, result);
            Assert.Equal("# Title", output);
            Assert.Equal("This is a comment", comment);
        }

        [Fact]
        public void UnclosedHtmlCommentStartIsFormatExceptionError()
        {
            string input = "# Title <!-- This is a comment";
            XmlCommentExtractorResult result = input.TryExtractXmlComment(out string? output, out string? comment);
            Assert.Equal(XmlCommentExtractorResult.Error, result);
        }

        [Fact]
        public void UnclosedHtmlCommentEndIsFormatExceptionError()
        {
            string input = "# Title This is a comment -->";
            XmlCommentExtractorResult result = input.TryExtractXmlComment(out string? output, out string? comment);
            Assert.Equal(XmlCommentExtractorResult.Error, result);
        }


        [Fact]
        public void ParagraphIsParsed()
        {
            string input = File.ReadAllText("./test-data/markdown/paragraph.md");
            XmlCommentExtractorResult result = input.TryExtractXmlComment(out string? output, out string? comment);
            Assert.Equal(XmlCommentExtractorResult.Comment, result);
            Assert.Equal("tags: ['paragraph']", comment);
            Assert.Equal("Artificial Intelligence (AI) refers to the simulation of human intelligence in machines that are programmed to think, learn, and perform tasks typically requiring human cognition. From simple rule-based systems to advanced machine learning models, AI has become a transformative force across industries, enhancing efficiency, decision-making, and automation. Its applications range from virtual assistants and recommendation systems to autonomous vehicles and medical diagnostics, shaping the future of technology and society.", output);
        }

        [Fact]
        public void ParagraphSpacedIsParsed()
        {
            string input = File.ReadAllText("./test-data/markdown/paragraph_spaced.md");
            XmlCommentExtractorResult result = input.TryExtractXmlComment(out string? output, out string? comment);
            Assert.Equal(XmlCommentExtractorResult.Comment, result);
            Assert.Equal("tags: ['paragraph']", comment);
            Assert.Equal("Artificial Intelligence (AI) refers to the simulation of human intelligence in machines that are programmed to think, learn, and perform tasks typically requiring human cognition. From simple rule-based systems to advanced machine learning models, AI has become a transformative force across industries, enhancing efficiency, decision-making, and automation. Its applications range from virtual assistants and recommendation systems to autonomous vehicles and medical diagnostics, shaping the future of technology and society.", output);
        }
    }
}
