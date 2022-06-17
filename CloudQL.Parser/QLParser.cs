using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static Pidgin.Parser<char, char>;

namespace CloudQL.QLParser
{
    public class Node
    {

    }

    public enum Keywords
    {
        Select,
        From,
        Where,
        Group,
        Order,
    }

    public enum Operators
    {
        Dot,
        Comma,
    }

    public class Resource
    {
        public string Name { get; set; } = string.Empty;
        public Resource? Child { get; set; }
    }

    public enum BinaryOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        And,
        Or,
    }

    public enum UnaryOperator
    {
        Not,
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

    public class BinaryExpression : Expression
    {
        public BinaryOperator Operator { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }
    }

    public class UnaryExpression : Expression
    {
        public UnaryOperator Operator { get; set; }
        public Expression Right { get; set; }
    }

    public abstract class Filter { }

    public class WhereFilter : Filter
    {
        public Expression Expression { get; set; }
    }

    public class Query
    {
        public Keywords Keyword { get; set; }

    }

    public static class QueryLanguage
    {

        public static readonly Parser<char, Keywords> Select = String("select").ThenReturn(Keywords.Select);
        public static readonly Parser<char, Keywords> From = String("from").ThenReturn(Keywords.From);
        //public static readonly Parser<char, Keywords> Where = String("where").ThenReturn(Keywords.Where);
        public static readonly Parser<char, BinaryOperator> And = String("and").ThenReturn(BinaryOperator.And);
        public static readonly Parser<char, BinaryOperator> Or = String("or").ThenReturn(BinaryOperator.Or);
        public static readonly Parser<char, UnaryOperator> Not = String("not").ThenReturn(UnaryOperator.Not);

        public static readonly Parser<char, Operators> Dot = Char('.').ThenReturn(Operators.Dot);
        public static readonly Parser<char, Operators> Comma = Char(',').ThenReturn(Operators.Comma);

        public static readonly Parser<char, string> Identifier = from first in Letter
                                                                 from rest in OneOf(LetterOrDigit, Char('_')).ManyString()
                                                                 select first + rest;
        public static readonly Parser<char, double> Float = from first in Digit.AtLeastOnceString()
                                                            from dot in Char('.')
                                                            from second in Digit.AtLeastOnceString()
                                                            select double.Parse(first + dot + second, System.Globalization.CultureInfo.InvariantCulture);

        public static readonly Parser<char, Resource> Resource = from ident in Identifier
                                                                 from child in Rec(() => Dot.Then(Resource)).Optional()
                                                                 select new Resource() { Name = ident, Child = child.GetValueOrDefault(() => null) };

        public static readonly Parser<char, Expression> IdentifierAtom = from ident in Identifier
                                                                         select new IdentifierExpression() { Identifier = ident } as Expression;
        public static readonly Parser<char, Expression> IntegerAtom = from integer in Digit.ManyString().Map(digits => long.Parse(digits))
                                                                      select new IntegerExpression() { Integer = integer } as Expression;
        public static readonly Parser<char, Expression> FloatAtom = from floatAtom in Float
                                                                    select new FloatExpression() { Float = floatAtom } as Expression;
        public static readonly Parser<char, Expression> Atom = OneOf(IdentifierAtom, Try(FloatAtom), Try(IntegerAtom));
        public static readonly Parser<char, Expression> Unary = OneOf(
                                                                    Try(
                                                                        from notOp in Not
                                                                        from expr in SkipWhitespaces.Then(Expression)
                                                                        select new UnaryExpression() { Operator = notOp, Right = expr } as Expression
                                                                    ),
                                                                    Atom);
        public static readonly Parser<char, Expression> Binary = OneOf(
                                                                    Try(
                                                                        from leftExpr in OneOf(Unary, Atom)
                                                                        from binOp in SkipWhitespaces.Then(OneOf(And, Or))
                                                                        from rightExpr in SkipWhitespaces.Then(Expression)
                                                                        select new BinaryExpression() { Left = leftExpr, Operator = binOp, Right = rightExpr } as Expression
                                                                    ),
                                                                    Unary
                                                                );
        public static readonly Parser<char, Expression> Expression = Binary;

        public static readonly Parser<char, Filter> Where = from whereKeyword in String("where")
                                                            from expr in Expression
                                                            select new WhereFilter() { Expression = expr } as Filter;

        public static readonly Parser<char, IEnumerable<Filter>> Filters = OneOf(Where).AtLeastOnce();


        public static readonly Parser<char, Query> Query = from fromKeyword in From
                                                           from filters in Filters.Optional()
                                                           from selectExpr in Select
                                                           select new Query() { };
    }
}
