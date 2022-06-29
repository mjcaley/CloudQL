using CloudQL.TestCloud;

namespace CloudQL.Engine.Tests
{
    public class ProviderAttributesTests
    {
        [Fact]
        public void ProviderAttributeReturnsAssembly()
        {
            var provider = ResourceResolver.GetProvider("test");

            Assert.NotNull(provider);
            Assert.IsType<TestProvider>(provider);
        }

        [Fact]
        public void ResourceAttributeReturns()
        {
            var resoureClient = ResourceResolver.GetResourceClient("test.resource");

            Assert.NotNull(resoureClient);
            Assert.IsType<TestResource>(resoureClient);
        }
    }
}
