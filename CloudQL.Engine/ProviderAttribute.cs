namespace CloudQL.Engine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ProviderAttribute : Attribute
    {
        public ProviderAttribute(string provider) => Provider = provider;

        public string Provider { get; private set; }
    }
}
