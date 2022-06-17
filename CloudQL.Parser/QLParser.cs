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

    public static class QueryLanguage
    {

        public static readonly Parser<char, Keywords> Select = String("select").ThenReturn(Keywords.Select);
        public static readonly Parser<char, Keywords> From = String("from").ThenReturn(Keywords.From);
        public static readonly Parser<char, Keywords> Where = String("where").ThenReturn(Keywords.Where);

        public static readonly Parser<char, Operators> Dot = Char('.').ThenReturn(Operators.Dot);
        public static readonly Parser<char, Operators> Comma = Char(',').ThenReturn(Operators.Comma);

        public static readonly Parser<char, string> Identifier = from first in Letter
                                                                 from rest in OneOf(LetterOrDigit, Char('_')).ManyString()
                                                                 select first + rest;

        public static readonly Parser<char, Resource> Resource = from ident in Identifier
                                                                 from child in Rec(() => Dot.Then(Resource)).Optional()
                                                                 select new Resource() { Name = ident, Child = child.GetValueOrDefault(() => null) };
    }
}
