namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutBinding.html">VkDescriptorSetLayoutBinding</see>
/// </summary>
public unsafe struct VkDescriptorSetLayoutBinding
{
    public uint               binding;
    public VkDescriptorType   descriptorType;
    public uint               descriptorCount;
    public VkShaderStageFlags stageFlags;
    public VkSampler*         pImmutableSamplers;
}
