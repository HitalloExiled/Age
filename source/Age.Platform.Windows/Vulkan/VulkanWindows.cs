using Age.Vulkan.Interfaces;
using Age.Vulkan.Native;

namespace Age.Platform.Windows.Vulkan;

public class VulkanWindows : Vk, IDisposable
{
    protected override IVulkanLoader Loader { get; } = new WindowsVulkanLoader();

    private bool disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            this.Loader.Dispose();
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
