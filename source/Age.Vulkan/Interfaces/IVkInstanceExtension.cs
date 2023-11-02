using Age.Vulkan.Native;
using Age.Vulkan.Native.Types;

namespace Age.Vulkan.Interfaces;

public interface IVkInstanceExtension
{
    static abstract string Name { get; }
    static abstract IVkInstanceExtension Create(Vk vk, VkInstance instance);
}
