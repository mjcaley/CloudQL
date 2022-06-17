using Pidgin;

namespace CloudQL.Parser.Tests
{
    public class QLParserTests
    {
        [Fact]
        public void ParsesResourceRecursive()
        {
            var result = QLParser.QueryLanguage.Resource.ParseOrThrow("azure.vm");

            Assert.Equal("azure", result.Name);
            Assert.NotNull(result.Child);
            Assert.Equal("vm", result.Child.Name);
            Assert.Null(result.Child.Child);
        }

        [Fact]
        public void ParsesResourceLeaf()
        {
            var result = QLParser.QueryLanguage.Resource.ParseOrThrow("azure");

            Assert.Equal("azure", result.Name);
            Assert.Null(result.Child);
        }
    }
}