using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Types;
using Age.Vulkan.Native.Types.KHR;

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
