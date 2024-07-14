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

    public Texture Texture { get; private set; }

    public VkFormat   Format => Texture.Image.Format;
    public Size<uint> Size   => Texture.Image.Extent.ToSize();

    public RenderTarget(in Size<uint> size, VkFormat format = VkFormat.B8G8R8A8Unorm) =>
        this.Update(size, format);

    private static (Framebuffer framebuffer, Texture texture) CreateResources(in Size<uint> size, VkFormat format)
    {
        if (RenderGraph.Active == null)
        {
            throw new InvalidOperationException("There no active RenderGraph");
        }

        var renderPass = RenderGraph.Active.GetRenderPass<SceneRenderGraphPass>()
            ?? throw new InvalidOperationException($"Can't find any {nameof(SceneRenderGraphPass)} on {RenderGraph.Active.Name} RenderGraph");

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
            RenderPass  = renderPass,
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

        var texture     = VulkanRenderer.Singleton.CreateTexture(resolveImage, true);
        var framebuffer = VulkanRenderer.Singleton.CreateFramebuffer(framebufferCreateInfo);

        return (framebuffer, texture);
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

        (Framebuffer, Texture) = CreateResources(size, format);
    }
}
