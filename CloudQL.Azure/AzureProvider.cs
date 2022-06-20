using CloudQL.Provider;

namespace CloudQL.Azure
{
    public class AzureProvider : ICloudProvider
    {
        public IResourceClient GetResourceClient(IEnumerable<string> resource)
        {
            throw new NotImplementedException();
        }
    }
}
