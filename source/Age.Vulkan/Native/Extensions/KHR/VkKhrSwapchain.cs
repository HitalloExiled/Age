using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Types;

namespace Age.Vulkan.Native.Extensions.KHR;

public unsafe class VkKhrSwapchain : IVkInstanceExtension
{
    public static string Name { get; } = "VK_KHR_swapchain";

    public VkKhrSwapchain(Vk vk, VkInstance instance)
    {

    }

    public static IVkInstanceExtension Create(Vk vk, VkInstance instance) =>
        new VkKhrSwapchain(vk, instance);
}
