using Age.Vulkan.Enums.KHR;
using Age.Vulkan.Types;
using Age.Vulkan.Types.KHR;

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
