using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutBinding.html">VkDescriptorSetLayoutBinding</see>
/// </summary>
public unsafe struct VkDescriptorSetLayoutBinding
{
    public uint                 Binding;
    public VkDescriptorType     DescriptorType;
    public uint                 DescriptorCount;
    public VkShaderStageFlags   StageFlags;
    public VkHandle<VkSampler>* PImmutableSamplers;
}
