using CloudQL.Provider;

namespace CloudQL.TestCloud
{
    [Provider("test")]
    public class TestProvider : ICloudProvider
    {
        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
