using Age.Numerics;
using Age.Rendering.Extensions;
using Age.Rendering.RenderPasses;
using Age.Rendering.Vulkan;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class RenderTarget : Resource
{
    internal Framebuffer Framebuffer { get; private set; }
    internal Image[]     Attachments { get; private set; } = new Image[3];

    public Texture Texture { get; private set; }

    public VkFormat   Format => this.Texture.Image.Format;
    public Size<uint> Size   => this.Texture.Image.Extent.ToSize();

    public RenderTarget(Texture texture, Framebuffer framebuffer, Image[] attachments)
    {
        this.Texture     = texture;
        this.Framebuffer = framebuffer;
        this.Attachments = attachments;
    }

    public RenderTarget(in Size<uint> size, VkFormat format = VkFormat.B8G8R8A8Unorm) =>
        this.Update(size, format);

    [MemberNotNull(nameof(Framebuffer), nameof(Texture))]
    private void CreateResources(in Size<uint> size, VkFormat format)
    {
        var pass = RenderGraph.Active.GetRenderGraphPass<SceneRenderGraphPass>();

        var colorImageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = size.ToExtent3D(),
            Format        = format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = VulkanRenderer.Singleton.MaxUsableSampleCount,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransientAttachment | VkImageUsageFlags.ColorAttachment,
        };

        var colorImage = VulkanRenderer.Singleton.CreateImage(colorImageCreateInfo);

        var resolveImageCreateInfo = colorImageCreateInfo;

        resolveImageCreateInfo.Usage   = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;
        resolveImageCreateInfo.Samples = VkSampleCountFlags.N1;

        var resolveImage = VulkanRenderer.Singleton.CreateImage(resolveImageCreateInfo);

        var depthImageCreateInfo = colorImageCreateInfo;

        depthImageCreateInfo.Usage  = VkImageUsageFlags.DepthStencilAttachment;
        depthImageCreateInfo.Format = VulkanRenderer.Singleton.DepthBufferFormat;

        var depthImage = VulkanRenderer.Singleton.CreateImage(depthImageCreateInfo);

        var framebufferCreateInfo = new FramebufferCreateInfo
        {
            RenderPass  = pass.RenderPass,
            Attachments =
            [
                new FramebufferCreateInfo.Attachment
                {
                    Image       = colorImage,
                    ImageAspect = VkImageAspectFlags.Color,
                },
                new FramebufferCreateInfo.Attachment
                {
                    Image       = resolveImage,
                    ImageAspect = VkImageAspectFlags.Color,
                },
                new FramebufferCreateInfo.Attachment
                {
                    Image       = depthImage,
                    ImageAspect = VkImageAspectFlags.Depth,
                },
            ]
        };

        this.Texture     = VulkanRenderer.Singleton.CreateTexture(resolveImage, true);
        this.Framebuffer = VulkanRenderer.Singleton.CreateFramebuffer(framebufferCreateInfo);

        this.Attachments[0] = colorImage;
        this.Attachments[1] = resolveImage;
        this.Attachments[2] = depthImage;
    }

    protected override void OnDispose()
    {
        this.Texture.Dispose();
        this.Framebuffer.Dispose();
    }

    [MemberNotNull(nameof(Framebuffer), nameof(Texture))]
    public void Update(in Size<uint> size, VkFormat format = VkFormat.B8G8R8A8Unorm)
    {
        this.Framebuffer?.Dispose();
        this.Texture?.Dispose();

        foreach (var attachment in this.Attachments)
        {
            attachment?.Dispose();
        }

        this.CreateResources(size, format);
    }
}
