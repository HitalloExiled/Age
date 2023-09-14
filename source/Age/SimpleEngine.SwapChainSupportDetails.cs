using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.KHR.Types;
using Age.Vulkan.Native.Types;

namespace Age;

public unsafe partial class SimpleEngine
{
    public record SwapChainSupportDetails
    {
        public required VkSurfaceCapabilitiesKHR Capabilities { get; init; }
        public required VkSurfaceFormatKHR[]     Formats      { get; init; }
        public required VkPresentModeKHR[]       PresentModes { get; init; }
    }
}
