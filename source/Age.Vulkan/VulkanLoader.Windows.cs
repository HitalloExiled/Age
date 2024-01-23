#if !Windows
#define Windows
#endif

using Age.Vulkan.Interfaces;

namespace Age.Vulkan;

public partial class VulkanLoader : IVulkanLoader
{
#if Windows
    private const string PLATFORM_PATH = "vulkan-1.dll";
#endif
}
