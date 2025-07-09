namespace XArch.StructuredMarkdown.UnitTests
{
    using Snapshooter.Xunit;

    using XArch.StructuredMarkdown.Serialization;

    public class PropertyBagTests
    {
        [Fact]
        public void CanParsePropertyBag()
        {
            File.ReadAllText("./test-data/metadata/raw.yml")
                .FromYaml<PropertyBag>()
                .MatchSnapshot();
        }

        [Fact]
        public void CanParseSerialziePropertyBag()
        {
            PropertyBag bag = new PropertyBag
            {
                { "title", "Test Document" },
                { "author", "John Doe" },
                { "tags", new List<string> { "test", "example" } }
            };

            bag.ToYaml()
                .MatchSnapshot();
        }
    }
}