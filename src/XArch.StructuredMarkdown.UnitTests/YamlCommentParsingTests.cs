namespace XArch.StructuredMarkdown.UnitTests
{
    using System.Diagnostics;

    using Snapshooter.Xunit;

    using XArch.StructuredMarkdown.Serialization;

    public class YamlCommentParsingTests
    {
        [Fact]
        public void CanParseStartSection()
        {
            string comment = "section: section-id";
            var result = Parsing.XmlCommentParser.TryParseMetadata(comment, out var propertyBag);

            Assert.Equal(Parsing.XmlCommentKind.StartSection, result);
            Assert.Equal("section-id", propertyBag?["section"]);
        }

        [Fact]
        public void CanParseEndSection()
        {
            string comment = "/section";
            var result = Parsing.XmlCommentParser.TryParseMetadata(comment, out var propertyBag);

            Assert.Equal(Parsing.XmlCommentKind.EndSection, result);
            Assert.Null(propertyBag);
        }

        [Fact]
        public void CanParseMetadataFromComment()
        {
            string comment = "key1: value1, key2: value2, key3: 3.0, key4: ['abc;', 'defg']";
            var metadata = Parsing.XmlCommentParser.ParseMetadata(comment);
            metadata.ToYaml().MatchSnapshot();
        }

        [Fact]
        public void CanParseMetadataFromCommentAndConvertback()
        {
            string comment = "key1: value1, key2: value2, key3: 3.0, key4: ['abc;', 'defg']";
            var metadata = Parsing.XmlCommentParser.ParseMetadata(comment);
            metadata.ToYaml().ToHtmlComment().MatchSnapshot();
        }

        /// <summary>
        /// TODO: Improve performance of parsing metadata from HTML comments.
        /// Ideally this should be under 100ms for 5000 iterations which is about 20 microseconds per iteration.
        /// </summary>
        [Fact]
        public void ValidateParsingPerformance()
        {
            Stopwatch timer = new Stopwatch();
            double totalIterations = 5000;
            double maxMilliseconds = 2000;
            double maxMillisecondsPerIteration = maxMilliseconds / totalIterations;

            timer.Start();
            for (int i = 0; i < totalIterations; i++)
            {
                
                string comment = "key1: value1, key2: value2, key3: 3.0, key4: ['abc;', 'defg']";
                var metadata = Parsing.XmlCommentParser.ParseMetadata(comment);
                metadata.ToYaml().ToHtmlComment();
            }

            timer.Stop();

            Assert.True((timer.ElapsedMilliseconds / totalIterations) < maxMillisecondsPerIteration, $"Parsing too too long: Total - {timer.ElapsedMilliseconds}ms, {timer.Elapsed.TotalMicroseconds / totalIterations} Microseconds/it");
        }
    }
}
