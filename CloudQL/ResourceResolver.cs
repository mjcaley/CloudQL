using CloudQL.Provider;
using System.Reflection;

namespace CloudQL
{
    public class ResourceResolver
    {
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
