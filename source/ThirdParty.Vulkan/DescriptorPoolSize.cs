using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorPoolSize.html">VkDescriptorPoolSize</see>
/// </summary>
public unsafe record DescriptorPoolSize : NativeReference<VkDescriptorPoolSize>
{
    public DescriptorType Type
    {
        get => this.PNative->type;
        init => this.PNative->type = value;
    }

    public uint DescriptorCount
    {
        get => this.PNative->descriptorCount;
        init => this.PNative->descriptorCount = value;
    }
}
