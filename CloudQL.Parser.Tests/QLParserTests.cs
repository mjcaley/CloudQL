using CloudQL.QLParser;
using Pidgin;

namespace CloudQL.Parser.Tests
{
    public class QLParserTests
    {
        [Fact]
        public void ParsesResourceRecursive()
        {
            var result = QueryLanguage.Resource.ParseOrThrow("azure.vm");

            Assert.Equal("azure", result.Name);
            Assert.NotNull(result.Child);
            Assert.Equal("vm", result.Child.Name);
            Assert.Null(result.Child.Child);
        }

        [Fact]
        public void ParsesResourceLeaf()
        {
            var result = QueryLanguage.Resource.ParseOrThrow("azure");

            Assert.Equal("azure", result.Name);
            Assert.Null(result.Child);
        }

        [Fact]
        public void ParsesIdentifierExpression()
        {
            var result = QueryLanguage.Expression.ParseOrThrow("ident");

            var atom = Assert.IsType<IdentifierExpression>(result);
            Assert.Equal("ident", atom.Identifier);
        }

        [Fact]
        public void ParsesIntegerExpression()
        {
            var result = QueryLanguage.Expression.ParseOrThrow("42");

            var atom = Assert.IsType<IntegerExpression>(result);
            Assert.Equal(42L, atom.Integer);
        }

        [Fact]
        public void ParsesFloatExpression()
        {
            var result = QueryLanguage.Expression.ParseOrThrow("42.42");

            var atom = Assert.IsType<FloatExpression>(result);
            Assert.Equal(42.42, atom.Float);
        }

        [Fact]
        public void ParsesUnaryExpression()
        {
            var result = QueryLanguage.Expression.ParseOrThrow("not ident");

            var expr = Assert.IsType<UnaryExpression>(result);
            Assert.Equal(UnaryOperator.Not, expr.Operator);
        }

        [Theory]
        [InlineData("and", BooleanOperator.And)]
        [InlineData("or", BooleanOperator.Or)]
        public void ParsesBooleanExpression(string testInput, BooleanOperator expected)
        {
            var result = QueryLanguage.Expression.ParseOrThrow($"42 {testInput} 24");

            var expr = Assert.IsType<BooleanExpression>(result);
            Assert.Equal(expected, expr.Operator);
            var left = expr.Left as IntegerExpression;
            Assert.Equal(42, left.Integer);
            var right = expr.Right as IntegerExpression;
            Assert.Equal(24, right.Integer);
        }
    }
}
