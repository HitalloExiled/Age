using Age.Vulkan.Types;

namespace Age.Vulkan.Interfaces;

public interface IVkDeviceExtension<T> where T : IVkDeviceExtension<T>
{
    static abstract string Name { get; }
    static abstract T Create(Vk vk, VkDevice device);
}
