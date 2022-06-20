namespace CloudQL.Provider
{
    public interface IResourceClient
    {
        void Initialize(ICloudProvider cloudProvider);
        void Get();
        void Set();
    }
}
