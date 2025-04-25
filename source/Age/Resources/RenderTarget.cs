using Age.Numerics;
using Age.Rendering.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using Age.Rendering.Resources;
using Age.RenderPasses;
using Age.Rendering.Vulkan;

using ImageResource   = Age.Rendering.Resources.Image;

namespace Age.Resources;

public sealed class RenderTarget : Resource
{
    internal Framebuffer     Framebuffer { get; private set; }
    internal ImageResource[] Attachments { get; private set; } = new ImageResource[3];

    public Texture2D Texture { get; private set; }
    public Size<uint> Size   => this.Texture.Size;

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
            Usage         = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled,
        };

        var colorImage = new ImageResource(colorImageCreateInfo);
        colorImage.ClearColor(Color.Margenta, VkImageLayout.ShaderReadOnlyOptimal);

        var resolveImageCreateInfo = colorImageCreateInfo;

        resolveImageCreateInfo.Usage   = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;
        resolveImageCreateInfo.Samples = VkSampleCountFlags.N1;

        var resolveImage = new ImageResource(resolveImageCreateInfo);
        resolveImage.ClearColor(Color.Margenta, VkImageLayout.ShaderReadOnlyOptimal);

        var depthImageCreateInfo = colorImageCreateInfo;

        depthImageCreateInfo.Usage  = VkImageUsageFlags.DepthStencilAttachment;
        depthImageCreateInfo.Format = VulkanRenderer.Singleton.DepthBufferFormat;

        var depthImage = new ImageResource(depthImageCreateInfo);

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

        this.Texture     = new(resolveImage);
        this.Framebuffer = new(framebufferCreateInfo);

        this.Attachments[0] = colorImage;
        this.Attachments[1] = resolveImage;
        this.Attachments[2] = depthImage;
    }

    protected override void OnDisposed()
    {
        this.Texture.Dispose();
        this.Framebuffer.Dispose();

        foreach (var attachments in this.Attachments)
        {
            attachments.Dispose();
        }
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
