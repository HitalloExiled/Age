using Age.Vulkan.Native;

namespace Age.Vulkan.Interfaces;

public interface IVkInstanceExtension
{
    static abstract string Name { get; }
    static abstract IVkInstanceExtension Create(Vk vk, VkInstance instance);
}
