using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace CloudQL.QLParser
{
    public static class QueryLanguage
    {
        private static Parser<char, T> Tok<T>(Parser<char, T> token) => Try(token).Before(SkipWhitespaces);
        private static Parser<char, string> Tok(string token) => Tok(String(token));
        
        private static readonly Parser<char, BooleanOperator> And = Tok("and").ThenReturn(BooleanOperator.And);
        private static readonly Parser<char, BooleanOperator> Or = Tok("or").ThenReturn(BooleanOperator.Or);
        public static readonly Parser<char, BooleanOperator> BoolOps = OneOf(And, Or);

        private static readonly Parser<char, UnaryOperator> Not = Tok("not").ThenReturn(UnaryOperator.Not);
        private static readonly Parser<char, UnaryOperator> Neg = Tok("-").ThenReturn(UnaryOperator.Negative);
        private static readonly Parser<char, UnaryOperator> Pos = Tok("+").ThenReturn(UnaryOperator.Positive);
        public static readonly Parser<char, UnaryOperator> UnaryOps = OneOf(Not, Neg, Pos);

        private static readonly Parser<char, ComparisonOperator> Equal = from _ in Tok("==")
                                                                        select ComparisonOperator.Equal;
        private static readonly Parser<char, ComparisonOperator> NotEqual = from _ in Tok("!=")
                                                                           select ComparisonOperator.NotEqual;
        private static readonly Parser<char, ComparisonOperator> LessThan = from _ in Tok("<")
                                                                           select ComparisonOperator.LessThan;
        private static readonly Parser<char, ComparisonOperator> GreaterThan = from _ in Tok(">")
                                                                              select ComparisonOperator.GreaterThan;
        private static readonly Parser<char, ComparisonOperator> LessThanOrEqual = from _ in Tok("<=")
                                                                                  select ComparisonOperator.LessThanOrEqual;
        private static readonly Parser<char, ComparisonOperator> GreaterThanOrEqual = from _ in Tok(">=")
                                                                                     select ComparisonOperator.GreaterThanOrEqual;
        public static readonly Parser<char, ComparisonOperator> CompOps = OneOf(
            Try(Equal), Try(NotEqual), Try(GreaterThanOrEqual), Try(LessThanOrEqual), Try(GreaterThan), Try(LessThan)
            );
         
        public static readonly Parser<char, string> Dot = Tok(".");
        public static readonly Parser<char, string> Comma = Tok(",");

        public static readonly Parser<char, string> Identifier = from first in Letter
                                                                 from rest in OneOf(LetterOrDigit, Char('_')).ManyString()
                                                                 select first + rest;

        public static readonly Parser<char, Resource> Resource = from idents in Identifier.Separated(Dot)
                                                                 select new Resource() { Names = idents.ToList() };

        public static readonly Parser<char, Expression> IdentifierAtom = from ident in Tok(Identifier)
                                                                         select new IdentifierExpression() { Identifier = ident } as Expression;
        public static readonly Parser<char, Expression> IntegerAtom = from integer in Tok(LongNum)
                                                                      select new IntegerExpression() { Integer = integer } as Expression;
        public static readonly Parser<char, Expression> StringAtom =
            AnyCharExcept('"').ManyString().Between(Char('"')).Map(s => new StringExpression() { Value = s } as Expression);

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
        public static readonly Parser<char, Expression> Atom = OneOf(IdentifierAtom, Try(FloatAtom), Try(IntegerAtom), StringAtom);
        public static readonly Parser<char, Expression> Unary = OneOf(
                                                                    Try(
                                                                        from unaryOp in UnaryOps
                                                                        from expr in Tok(Expression)
                                                                        select new UnaryExpression() { Operator = unaryOp, Right = expr } as Expression
                                                                    ),
                                                                    Atom);
        public static readonly Parser<char, Expression> Comparison = OneOf(
                                                                        Try(
                                                                            from leftExpr in Unary
                                                                            from compOp in CompOps
                                                                            from rightExpr in Tok(Expression)
                                                                            select new ComparisonExpression() { Operator = compOp, Left = leftExpr, Right = rightExpr } as Expression
                                                                        ),
                                                                        Unary
                                                                        );
        public static readonly Parser<char, Expression> Boolean = OneOf(
                                                                    Try(
                                                                        from leftExpr in Comparison
                                                                        from boolOp in BoolOps
                                                                        from rightExpr in Tok(Expression)
                                                                        select new BooleanExpression() { Left = leftExpr, Operator = boolOp, Right = rightExpr } as Expression
                                                                    ),
                                                                    Comparison
                                                                );
        public static readonly Parser<char, Expression> Expression = Boolean;

        public static readonly Parser<char, Filter> SelectClause = from keyword in Tok("select")
                                                                   from columns in Identifier.Separated(Comma)
                                                                   select new SelectFilter() { Columns = columns } as Filter;

        public static readonly Parser<char, Filter> SortClause = from keyword in Tok("sort")
                                                                 from direction in OneOf(Tok("asc"), Tok("desc")).Optional().Map(s =>
                                                                 {
                                                                     if (s.HasValue)
                                                                     {
                                                                         return s.Value switch
                                                                         {
                                                                             "asc" => SortOrder.Ascending,
                                                                             "desc" => SortOrder.Descending,
                                                                             _ => SortOrder.Ascending,
                                                                         };
                                                                     }

                                                                     return SortOrder.Ascending;
                                                                 })
                                                                 from columns in Identifier.Separated(Comma)
                                                                 select new SortFilter() { Columns = columns, Direction = direction } as Filter;

        public static readonly Parser<char, Filter> WhereClause = from keyword in Tok("where")
                                                                  from expr in Tok(Expression)
                                                                  select new WhereFilter() { Expression = expr } as Filter;

        public static readonly Parser<char, Filter> Filter = OneOf(SelectClause, SortClause, WhereClause);

        public static readonly Parser<char, Query> Query = from fromKeyword in Tok("from")
                                                           from resource in Tok(Resource)
                                                           from filters in Filter.Many()
                                                           select new Query() { Resource = resource, Filters = filters };
    }
}
