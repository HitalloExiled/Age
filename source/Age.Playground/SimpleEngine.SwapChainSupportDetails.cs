using Age.Vulkan.Native.Enums.KHR;
using Age.Vulkan.Native.Types;
using Age.Vulkan.Native.Types.KHR;

namespace Age.Playground;

public unsafe partial class SimpleEngine
{
    public record SwapChainSupportDetails
    {
        public required VkSurfaceCapabilitiesKHR Capabilities { get; init; }
        public required VkSurfaceFormatKHR[]     Formats      { get; init; }
        public required VkPresentModeKHR[]       PresentModes { get; init; }
    }
}
