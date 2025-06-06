using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public sealed class Sampler : Resource
{
    public VkSampler Value { get; }

    internal Sampler()
    {
        VulkanRenderer.Singleton.Context.Device.PhysicalDevice.GetProperties(out var properties);

        var createInfo = new VkSamplerCreateInfo
        {
            AddressModeU  = VkSamplerAddressMode.Repeat,
            AddressModeV  = VkSamplerAddressMode.Repeat,
            AddressModeW  = VkSamplerAddressMode.Repeat,
            BorderColor   = VkBorderColor.IntOpaqueBlack,
            CompareOp     = VkCompareOp.Always,
            MagFilter     = VkFilter.Linear,
            MaxAnisotropy = properties.Limits.MaxSamplerAnisotropy,
            MaxLod        = 1,
            MinFilter     = VkFilter.Linear,
            MipmapMode    = VkSamplerMipmapMode.Linear,
        };

        this.Value = VulkanRenderer.Singleton.Context.Device.CreateSampler(createInfo);
    }

    protected override void OnDisposed() =>
        this.Value.Dispose();

    public static implicit operator VkSampler(Sampler value) => value.Value;
}
