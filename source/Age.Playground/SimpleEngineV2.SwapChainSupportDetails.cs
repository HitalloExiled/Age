using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.KHR;

namespace Age.Playground;

public unsafe partial class SimpleEngineV2
{
    public record SwapChainSupportDetails
    {
        public required SurfaceCapabilities Capabilities { get; init; }
        public required SurfaceFormat[]     Formats      { get; init; }
        public required PresentMode[]       PresentModes { get; init; }
    }
}
