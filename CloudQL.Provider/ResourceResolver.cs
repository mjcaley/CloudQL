using System.Reflection;

namespace CloudQL.Provider
{
    public static class ResourceResolver
    {
        private static Dictionary<string, Type> _providers = new();
        private static Dictionary<string, Type> _resourceClients = new();

        static ResourceResolver()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var providerAttribute = type.GetCustomAttribute<ProviderAttribute>();
                    if (providerAttribute != null
                        && type.IsAssignableTo(typeof(ICloudProvider)))
                    {
                        _providers.Add(providerAttribute.Provider, type);
                    }

                    var resourceClientAttribute = type.GetCustomAttribute<ResourceClientAttribute>();
                    if (resourceClientAttribute != null
                        && type.IsAssignableTo(typeof(IResourceClient)))
                    {
                        _resourceClients.Add(string.Join('.', resourceClientAttribute.Resource), type);
                    }
                }
            }
        }

        public static ICloudProvider? GetProvider(string name)
        {
            var providerType = _providers.GetValueOrDefault(name, null);
            if (providerType == null)
            {
                return null;
            }

            var provider = (ICloudProvider)Activator.CreateInstance(providerType);

            return provider;
        }

        public static IResourceClient? GetResourceClient(string resource)
        {
            var resourceType = _resourceClients.GetValueOrDefault(resource, null);
            if (resourceType == null)
            {
                return null;
            }

            var resourceClient = (IResourceClient)Activator.CreateInstance(resourceType);

            return resourceClient;
        }
    }
}
