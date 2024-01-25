using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorImageInfo.html">VkDescriptorImageInfo</see>
/// </summary>
public unsafe record DescriptorImageInfo : NativeReference<VkDescriptorImageInfo>
{
    private Sampler?   sampler;
    private ImageView? imageView;

    public Sampler? Sampler
    {
        get => this.sampler;
        init => this.PNative->sampler = this.sampler = value;
    }

    public ImageView? ImageView
    {
        get => this.imageView;
        init => this.PNative->imageView = this.imageView = value;
    }

    public ImageLayout ImageLayout
    {
        get => this.PNative->imageLayout;
        init => this.PNative->imageLayout = value;
    }
}
