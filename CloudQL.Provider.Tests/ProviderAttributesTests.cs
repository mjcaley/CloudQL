namespace CloudQL.Provider.Tests
{
    public class ProviderAttributesTests
    {
        [Fact]
        public void ProviderAttributeReturnsAssembly()
        {
            var assembly = ResourceResolver.GetProvider("test");

            Assert.NotNull(assembly);
            Assert.Equal("CloudQL.TestProvider", assembly?.GetName().Name);
        }

        [Fact]
        public void ResourceAttributeReturns()
        {
            var assembly = ResourceResolver.GetProvider("test");
            var resoureClient = ResourceResolver.GetResourceClient(assembly, new[] { "vm" });

            Assert.NotNull(resoureClient);

        }
    }
}
