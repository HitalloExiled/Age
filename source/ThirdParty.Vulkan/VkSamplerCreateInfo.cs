using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkSamplerCreateInfo.html">VkSamplerCreateInfo</see>
/// </summary>
public unsafe struct VkSamplerCreateInfo
{
    public readonly VkStructureType SType;

    public void*                PNext;
    public VkSamplerCreateFlags Flags;
    public VkFilter             MagFilter;
    public VkFilter             MinFilter;
    public VkSamplerMipmapMode  MipmapMode;
    public VkSamplerAddressMode AddressModeU;
    public VkSamplerAddressMode AddressModeV;
    public VkSamplerAddressMode AddressModeW;
    public float                MipLodBias;
    public VkBool32             AnisotropyEnable;
    public float                MaxAnisotropy;
    public VkBool32             CompareEnable;
    public VkCompareOp          CompareOp;
    public float                MinLod;
    public float                MaxLod;
    public VkBorderColor        BorderColor;
    public VkBool32             UnnormalizedCoordinates;

    public VkSamplerCreateInfo() =>
        this.SType = VkStructureType.SamplerCreateInfo;
}
