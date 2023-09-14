namespace Age.Vulkan.Native.Extensions;

public interface IVkExtension
{
    public delegate T? GetInstanceProcAddr<T>(VkInstance instance, string name);

    static abstract string Name { get; }
    static abstract IVkExtension Create(Vk vk, VkInstance instance);
}
