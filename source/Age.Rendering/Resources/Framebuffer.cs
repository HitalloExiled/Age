using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public sealed class Framebuffer : Resource<VkFramebuffer>
{
    public VkExtent2D    Extent     { get; }
    public VkImageView[] ImageViews { get; }

    public override VkFramebuffer Instance { get; }

    public Framebuffer(in FramebufferCreateInfo createInfo)
    {
        var imageViews = new VkImageView[createInfo.Attachments.Length];

        for (var i = 0; i < createInfo.Attachments.Length; i++)
        {
            imageViews[i] = CreateImageView(createInfo.Attachments[i].Image, createInfo.Attachments[i].ImageAspect);
        }

        var extent = new VkExtent2D
        {
            Width  = createInfo.Attachments[0].Image.Extent.Width,
            Height = createInfo.Attachments[0].Image.Extent.Height,
        };

        this.Instance   = VulkanRenderer.Singleton.Context.CreateFrameBuffer(createInfo.RenderPass.Instance, imageViews.AsSpan(), extent);
        this.ImageViews = imageViews;
        this.Extent     = extent;
    }

    private static VkImageView CreateImageView(Image image, VkImageAspectFlags aspectMask)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            Format           = image.Format,
            Image            = image.Instance.Handle,
            SubresourceRange = new()
            {
                AspectMask = aspectMask,
                LayerCount = 1,
                LevelCount = 1,
            },
            ViewType = VkImageViewType.N2D,
        };

        return VulkanRenderer.Singleton.Context.Device.CreateImageView(imageViewCreateInfo);
    }

    protected override void Disposed()
    {
        this.Instance.Dispose();

        foreach (var imageView in this.ImageViews)
        {
            imageView.Dispose();
        }
    }
}
