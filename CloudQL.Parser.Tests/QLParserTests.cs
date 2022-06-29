using CloudQL.QLParser.Data;
using Pidgin;

namespace CloudQL.QLParser.Tests
{
    public class QLParserTests
    {
        [Fact]
        public void ParsesResources()
        {
            var result = QueryLanguage.Resource.ParseOrThrow("azure.vm");

            Assert.Equal("azure", result.Names[0]);
            Assert.Equal("vm", result.Names[1]);
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

        [Fact]
        public void ParsesStringExpression()
        {
            var result = QueryLanguage.Expression.ParseOrThrow(@"""A string value""");

            var expr = Assert.IsType<StringExpression>(result);
            Assert.Equal("A string value", expr.Value);
        }

        [Theory]
        [InlineData("and", BooleanOperator.And)]
        [InlineData("or", BooleanOperator.Or)]
        public void ParsesBooleanExpression(string testInput, BooleanOperator expected)
        {
            var result = QueryLanguage.Expression.ParseOrThrow($"42 {testInput} 24");

            var expr = Assert.IsType<BooleanExpression>(result);
            Assert.Equal(expected, expr.Operator);
            var left = Assert.IsType<IntegerExpression>(expr.Left);
            Assert.Equal(42, left.Integer);
            var right = Assert.IsType<IntegerExpression>(expr.Right);
            Assert.Equal(24, right.Integer);
        }

        [Theory]
        [InlineData("==", ComparisonOperator.Equal)]
        [InlineData("!=", ComparisonOperator.NotEqual)]
        [InlineData(">", ComparisonOperator.GreaterThan)]
        [InlineData("<", ComparisonOperator.LessThan)]
        [InlineData(">=", ComparisonOperator.GreaterThanOrEqual)]
        [InlineData("<=", ComparisonOperator.LessThanOrEqual)]
        public void ParsesComparisonExpression(string testInput, ComparisonOperator expected)
        {
            var result = QueryLanguage.Expression.ParseOrThrow($"42 {testInput} 24");

            var expr = Assert.IsType<ComparisonExpression>(result);
            Assert.Equal(expected, expr.Operator);
            Assert.IsType<IntegerExpression>(expr.Left);
            Assert.IsType<IntegerExpression>(expr.Right);
        }

        [Fact]
        public void ParsesComplexExpression()
        {
            var result = QueryLanguage.Expression.ParseOrThrow("one == 1 and not 2.2");

            Assert.NotNull(result);
        }

        [Fact]
        public void ParsesSelectClauseOneColumn()
        {
            var result = QueryLanguage.Filter.ParseOrThrow("select one");

            var select = Assert.IsType<SelectFilter>(result);
            Assert.Equal(new[] { "one" }, select.Columns);
        }

        [Fact]
        public void ParsesSelectClauseMultipleColumns()
        {
            var result = QueryLanguage.Filter.ParseOrThrow("select one,two,three");

            var select = Assert.IsType<SelectFilter>(result);
            Assert.Equal(new[] { "one", "two", "three" }, select.Columns);
        }

        [Theory]
        [InlineData("asc", SortOrder.Ascending)]
        [InlineData("desc", SortOrder.Descending)]
        [InlineData("", SortOrder.Ascending)]
        public void ParsesSortClauseOneColumn(string testInput, SortOrder expected)
        {
            var result = QueryLanguage.Filter.ParseOrThrow($"sort {testInput} one");

            var filter = Assert.IsType<SortFilter>(result);
            Assert.Equal(expected, filter.Direction);
            Assert.Equal(new[] { "one" }, filter.Columns);
        }

        [Theory]
        [InlineData("asc", SortOrder.Ascending)]
        [InlineData("desc", SortOrder.Descending)]
        [InlineData("", SortOrder.Ascending)]
        public void ParsesSortClauseMultipleColumns(string testInput, SortOrder expected)
        {
            var result = QueryLanguage.Filter.ParseOrThrow($"sort {testInput} one, two, three");

            var filter = Assert.IsType<SortFilter>(result);
            Assert.Equal(expected, filter.Direction);
            Assert.Equal(new[] { "one", "two", "three" }, filter.Columns);
        }


        [Fact]
        public void ParsesWhereClause()
        {
            var result = QueryLanguage.Filter.ParseOrThrow("where 1 == 1");

            var where = Assert.IsType<WhereFilter>(result);
            Assert.NotNull(where.Expression);
        }

        [Fact]
        public void ParsesQuery()
        {
            var result = QueryLanguage.Query.ParseOrThrow("from azure.vm where name == 42 select name");

            Assert.NotNull(result);
        }

        [Fact]
        public void ParsesAssignment()
        {
            var result = QueryLanguage.Assignment.ParseOrThrow("let ident = from azure.vm select name;");

            Assert.NotNull(result);
            Assert.Equal("ident", result.Identifier);
            Assert.NotNull(result.Query);
        }
    }
}
