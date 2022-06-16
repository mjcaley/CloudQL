using Pidgin;

namespace CloudQL.Parser.Tests
{
    public class QLParserTests
    {
        [Fact]
        public void ParsesResources()
        {
            var result = QLParser.QueryLanguage.Resource.ParseOrThrow("azure.vm");

            Assert.Equal("azure", result.Name);
            Assert.NotNull(result.Child);
            Assert.Equal("vm", result.Child.Name);
            Assert.Null(result.Child.Child);
        }
    }
}