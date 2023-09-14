using Age.Platform.Windows.Display;
using Age.Platform.Windows.Vulkan;
using Age.Vulkan.Native;

using var loader = new WindowsVulkanLoader();

var vk = new Vulkan(loader);

_ = vk.EnumerateInstanceExtensionProperties(null, out var extensionCount, default);

unsafe
{
    fixed (char* pApplicationName = "Hello Triangle")
    fixed (char* pEngineName      = "No Engine")
    {
        var appInfo = new VkApplicationInfo
        {
            sType              = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
            pApplicationName   = pApplicationName,
            applicationVersion = Vulkan.MakeApiVersion(1, 0, 0, 0),
            pEngineName        = pEngineName,
            engineVersion      = Vulkan.MakeApiVersion(1, 0, 0, 0),
            apiVersion         = Vulkan.ApiVersion1_0,
        };

        var createInfo = new VkInstanceCreateInfo
        {
            sType            = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
            pApplicationInfo = &appInfo,
        };

        var result = vk.CreateInstance(createInfo, default, out var instance);
    }
}

using var windowManager = new WindowManager();

var window = windowManager.CreateWindow("Window1", 600, 400, 0, 0);

window.Show();

while (!window.Closed)
{
    window.DoEvents();
}
