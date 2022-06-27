namespace CloudQL.Provider
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ResourceClientAttribute : Attribute
    {
        public ResourceClientAttribute(params string[] resource) => Resource = resource;

        public string[] Resource { get; private set; }
    }
}
