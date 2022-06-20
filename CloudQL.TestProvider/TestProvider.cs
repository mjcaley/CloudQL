using CloudQL.Provider;

namespace CloudQL.TestProvider
{
    [Provider("test")]
    public class TestProvider : ICloudProvider
    {
        public IResourceClient GetResourceClient(IEnumerable<string> resource)
        {
            throw new NotImplementedException();
        }
    }
}