namespace CloudQL.QLParser.Data
{
    public class Query
    {
        public Resource Resource { get; set; }
        public IEnumerable<Filter> Filters { get; set; }
    }
}
