using CloudQL.Provider;

namespace CloudQL.TestCloud
{
    [ResourceClient("test", "resource")]
    public class TestResource : IResourceClient
    {
        public void Get()
        {
            throw new NotImplementedException();
        }

        public void Initialize(ICloudProvider cloudProvider)
        {
            throw new NotImplementedException();
        }

        public void Set()
        {
            throw new NotImplementedException();
        }
    }
}
