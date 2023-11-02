namespace Age.Vulkan.Interfaces;

public interface IVulkanLoader : IDisposable
{
    T Load<T>(string name) where T : Delegate;
    bool TryLoad<T>(string name, out T? result) where T : Delegate;
}
