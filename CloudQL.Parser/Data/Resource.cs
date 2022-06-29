namespace CloudQL.QLParser.Data
{
    public class Resource
    {
        public string Name { get; set; } = string.Empty;
        public Resource? Child { get; set; }
    }
}
