using Age.Vulkan.Types;

namespace Age.Vulkan.Interfaces;

public interface IVkDeviceExtension
{
    static abstract string Name { get; }
    static abstract IVkDeviceExtension Create(Vk vk, VkDevice device);
}
