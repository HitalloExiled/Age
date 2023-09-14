using Age.Core.Extensions;
using Age.Platform.Windows.Display;
using Age.Platform.Windows.Vulkan;
using Age.Vulkan.Native;

namespace Age;

public class SimpleEngine : IDisposable
{
    private readonly VulkanWindows vk            = new();
    private readonly WindowManager windowManager = new();

    private bool       disposed;
    private VkInstance instance;
    private Window?    window;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            this.vk.Dispose();
            this.windowManager.Dispose();

            this.disposed = true;
        }
    }

    public unsafe void Cleanup() =>
        this.vk.DestroyInstance(this.instance, null);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    private unsafe void CreateInstance()
    {
        fixed (byte* pApplicationName = "Hello Triangle".ToUTF8Bytes())
        fixed (byte* pEngineName      = "No Engine".ToUTF8Bytes())
        {
            var appInfo = new VkApplicationInfo
            {
                sType              = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
                pApplicationName   = pApplicationName,
                applicationVersion = VK.MakeApiVersion(1, 0, 0),
                pEngineName        = pEngineName,
                engineVersion      = VK.MakeApiVersion(1, 0, 0),
                apiVersion         = VK.ApiVersion_1_0,
            };

            var createInfo = new VkInstanceCreateInfo
            {
                sType             = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
                pApplicationInfo  = &appInfo,
                enabledLayerCount = 0,
            };

            if (this.vk.CreateInstance(createInfo, default, out var instance) == VkResult.VK_SUCCESS)
            {
                this.instance = instance;
            }
        }
    }

    private void InitVulkan() =>
        this.CreateInstance();

    private void MainLoop()
    {
        this.window = this.windowManager.CreateWindow("Age", 600, 400, 0, 0);

        this.window.Show();

        while (!this.window.Closed)
        {
            this.window.DoEvents();
        }
    }

    public void Run()
    {
        this.InitVulkan();
        this.MainLoop();
        this.Cleanup();
    }
}
