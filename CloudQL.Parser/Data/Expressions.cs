namespace CloudQL.QLParser.Data
{
    public enum BooleanOperator
    {
        And,
        Or,
    }

    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
    }

    public enum UnaryOperator
    {
        Not,
        Negative,
        Positive,
    }

    public abstract class Expression { }

    public class IntegerExpression : Expression
    {
        public long Integer { get; set; }
    }

    public class FloatExpression : Expression
    {
        public double Float { get; set; }
    }

    public class IdentifierExpression : Expression
    {
        public string Identifier { get; set; }
    }

    public class StringExpression : Expression
    {
        public string Value { get; set; }
    }

    public class BooleanExpression : Expression
    {
        public BooleanOperator Operator { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    public class ComparisonExpression : Expression
    {
        public ComparisonOperator Operator { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    public class UnaryExpression : Expression
    {
        public UnaryOperator Operator { get; set; }
        public Expression Right { get; set; }
    }
}
