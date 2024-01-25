using ThirdParty.Vulkan.Interfaces;
using ThirdParty.Vulkan.KHR;

namespace ThirdParty.Vulkan.Extensions.KHR;

public class SwapchainExtension : IDeviceExtension<SwapchainExtension>
{
    public static string Name => throw new NotImplementedException();

    static SwapchainExtension IDeviceExtension<SwapchainExtension>.Create(Device device) => throw new NotImplementedException();

    public Swapchain CreateSwapchain(Swapchain.CreateInfo createInfo) => throw new NotImplementedException();
    public void QueuePresent(Queue presentQueue, PresentInfo presentInfo) => throw new NotImplementedException();
}
