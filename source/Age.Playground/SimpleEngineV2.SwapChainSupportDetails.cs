using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan;

namespace Age.Playground;

public unsafe partial class SimpleEngineV2
{
    public record SwapChainSupportDetails
    {
        public required VkSurfaceCapabilitiesKHR Capabilities { get; init; }
        public required VkSurfaceFormatKHR[]     Formats      { get; init; }
        public required VkPresentModeKHR[]       PresentModes { get; init; }
    }
}
