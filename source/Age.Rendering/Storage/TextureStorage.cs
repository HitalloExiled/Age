using Age.Rendering.Vulkan;

namespace Age.Rendering.Storage;

public class TextureStorage : IDisposable
{
    private bool disposed;
    private readonly VulkanRenderer renderer;

    public TextureStorage(VulkanRenderer renderer) =>
        this.renderer = renderer;

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;
        }
    }
}
