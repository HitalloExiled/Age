using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutBinding.html">VkDescriptorSetLayoutBinding</see>
/// </summary>
public unsafe record DescriptorSetLayoutBinding : NativeReference<VkDescriptorSetLayoutBinding>
{
    private Sampler[] immutableSamplers = [];

    public uint Binding
    {
        get => this.PNative->binding;
        init => this.PNative->binding = value;
    }

    public DescriptorType DescriptorType
    {
        get => this.PNative->descriptorType;
        init => this.PNative->descriptorType = value;
    }

    public uint DescriptorCount
    {
        get => this.PNative->descriptorCount;
        init => this.PNative->descriptorCount = value;
    }

    public ShaderStageFlags StageFlags
    {
        get => this.PNative->stageFlags;
        init => this.PNative->stageFlags = value;
    }

    public Sampler[] ImmutableSamplers
    {
        get => this.immutableSamplers;
        init => Init(ref this.immutableSamplers, ref this.PNative->pImmutableSamplers, value);
    }

    protected override void OnFinalize() =>
        Free(ref this.PNative->pImmutableSamplers);
}
