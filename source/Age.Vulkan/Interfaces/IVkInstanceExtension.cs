using Age.Vulkan.Types;

namespace Age.Vulkan.Interfaces;

public interface IVkInstanceExtension<T> where T : IVkInstanceExtension<T>
{
    static abstract string Name { get; }
    static abstract T Create(Vk vk, VkInstance instance);
}
