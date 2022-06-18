namespace CloudQL.QLParser
{
    public abstract class Filter { }

    public class SelectFilter : Filter
    {
        public IEnumerable<string> Columns { get; set; }
    }

    public enum SortOrder
    {
        Ascending,
        Descending,
    }

    public class SortFilter : Filter
    {
        public SortOrder Direction { get; set; } = SortOrder.Ascending;
        public IEnumerable<string> Columns { get; set; }
    }

    public class WhereFilter : Filter
    {
        public Expression Expression { get; set; }
    }
}
