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
        private static Parser<char, T> Tok<T>(Parser<char, T> token) => Try(token).Before(SkipWhitespaces);
        private static Parser<char, string> Tok(string token) => Tok(String(token));

        public static readonly Parser<char, Keywords> From = Tok("from").ThenReturn(Keywords.From);
        public static readonly Parser<char, BooleanOperator> And = Tok("and").ThenReturn(BooleanOperator.And);
        public static readonly Parser<char, BooleanOperator> Or = Tok("or").ThenReturn(BooleanOperator.Or);
        public static readonly Parser<char, UnaryOperator> Not = Tok("not").ThenReturn(UnaryOperator.Not);
        public static readonly Parser<char, ComparisonOperator> Equal = from _ in Tok("==")
                                                                        select ComparisonOperator.Equal;
        public static readonly Parser<char, ComparisonOperator> NotEqual = from _ in Tok("!=")
                                                                           select ComparisonOperator.NotEqual;
        public static readonly Parser<char, ComparisonOperator> LessThan = from _ in Tok("<")
                                                                           select ComparisonOperator.LessThan;
        public static readonly Parser<char, ComparisonOperator> GreaterThan = from _ in Tok(">")
                                                                              select ComparisonOperator.GreaterThan;
        public static readonly Parser<char, ComparisonOperator> LessThanOrEqual = from _ in Tok("<=")
                                                                                  select ComparisonOperator.LessThanOrEqual;
        public static readonly Parser<char, ComparisonOperator> GreaterThanOrEqual = from _ in Tok(">=")
                                                                                     select ComparisonOperator.GreaterThanOrEqual;
         
        public static readonly Parser<char, Operators> Dot = Tok(".").ThenReturn(Operators.Dot);
        public static readonly Parser<char, Operators> Comma = Tok(",").ThenReturn(Operators.Comma);

        public static readonly Parser<char, string> Identifier = from first in Letter
                                                                 from rest in OneOf(LetterOrDigit, Char('_')).ManyString()
                                                                 select first + rest;

        public static readonly Parser<char, Resource> Resource = from ident in Identifier
                                                                 from child in Rec(() => Dot.Then(Resource)).Optional()
                                                                 select new Resource() { Name = ident, Child = child.GetValueOrDefault(() => null) };

        public static readonly Parser<char, Expression> IdentifierAtom = from ident in Tok(Identifier)
                                                                         select new IdentifierExpression() { Identifier = ident } as Expression;
        public static readonly Parser<char, Expression> IntegerAtom = from integer in Tok(LongNum)
                                                                      select new IntegerExpression() { Integer = integer } as Expression;

        private static readonly Parser<char, string> FloatFirst = OneOf(
            String("0"),
            Digit.Where(c => c != '0').Then(Digit.ManyString())
            );
        public static readonly Parser<char, double> FloatLiteral = FloatFirst.Then(Char('.')).Then(Digit.Many()).MapWithInput(
            (span, _) =>
            {
                var success = double.TryParse(span, out var result);
                if (success)
                {
                    return (double?)result;
                }
                return null;
            }
            )
            .Assert(result => result.HasValue)
            .Select(result => result!.Value);
                                                                   
        public static readonly Parser<char, Expression> FloatAtom = from floatAtom in Tok(FloatLiteral)
                                                                    select new FloatExpression() { Float = floatAtom } as Expression;
        public static readonly Parser<char, Expression> Atom = OneOf(IdentifierAtom, Try(FloatAtom), Try(IntegerAtom));
        public static readonly Parser<char, Expression> Unary = OneOf(
                                                                    Try(
                                                                        from notOp in Not
                                                                        from expr in Tok(Expression)
                                                                        select new UnaryExpression() { Operator = notOp, Right = expr } as Expression
                                                                    ),
                                                                    Atom);
        public static readonly Parser<char, Expression> Comparison = OneOf(
                                                                        Try(
                                                                            from leftExpr in Unary
                                                                            from compOp in OneOf(Try(Equal), Try(NotEqual), Try(LessThanOrEqual), Try(GreaterThanOrEqual), Try(LessThan), Try(GreaterThan))
                                                                            from rightExpr in Tok(Expression)
                                                                            select new ComparisonExpression() { Operator = compOp, Left = leftExpr, Right = rightExpr } as Expression
                                                                        ),
                                                                        Unary
                                                                        );
        public static readonly Parser<char, Expression> Boolean = OneOf(
                                                                    Try(
                                                                        from leftExpr in Comparison
                                                                        from boolOp in OneOf(And, Or)
                                                                        from rightExpr in Tok(Expression)
                                                                        select new BooleanExpression() { Left = leftExpr, Operator = boolOp, Right = rightExpr } as Expression
                                                                    ),
                                                                    Comparison
                                                                );
        public static readonly Parser<char, Expression> Expression = Boolean;

        public static readonly Parser<char, Filter> Where = from whereKeyword in Tok("where")
                                                            from expr in Tok(Expression)
                                                            select new WhereFilter() { Expression = expr } as Filter;

        //public static readonly Parser<char, Filter> Select = from selectKeyword in Tok("select")
        //                                                     from 

        public static readonly Parser<char, IEnumerable<Filter>> Filters = OneOf(Where).AtLeastOnce();


        public static readonly Parser<char, Query> Query = from fromKeyword in From
                                                           from resource in Tok(Resource)
                                                           from filters in Tok(Filters.Optional())
                                                           from selectExpr in Tok("select")
                                                           select new Query() { };
    }
}
