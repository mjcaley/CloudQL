using System.Reflection;

namespace CloudQL.Provider
{
    public static class ResourceResolver
    {
        private static Dictionary<string, ICloudProvider> _providers = new();
        private static Dictionary<string, IResourceClient> _resourceClients = new();

        static ResourceResolver()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var providerAttribute = type.GetCustomAttribute<ProviderAttribute>()
                    if (providerAttribute != null)
                    {
                        _providers.Add(providerAttribute.Provider, (ICloudProvider)type);
                    }
                }
            }
        }

        public static Assembly? GetProvider(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attribute = (ProviderAttribute?)assembly.GetCustomAttribute(typeof(ProviderAttribute));
                if (attribute?.Provider == name)
                {
                    return assembly;
                }
            }

            return null;
        }

        public static IResourceClient? GetResourceClient(Assembly assembly, IEnumerable<string> resource)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attributes = (ResourceClientAttribute[])type.GetCustomAttributes(typeof(ResourceClientAttribute), false);
                var resourceAttributes = attributes.Select(a => a.Resource);
                if (resource == resourceAttributes && type is IResourceClient)
                {
                    return type! as IResourceClient;
                }
            }

            return null;
        }
    }
}
