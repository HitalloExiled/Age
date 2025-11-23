using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Core;
using Age.Numerics;
using Age.Rendering.Extensions;
using Age.Rendering.Vulkan;
using System.Diagnostics.CodeAnalysis;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget : Resource
{
    private static readonly Dictionary<int, SharedResource<VkRenderPass>> renderPasses = [];

    private readonly List<ColorAttachment> colorAttachments = [];

    private readonly SharedResource<VkRenderPass> renderPass;

    internal VkFramebuffer Framebuffer { get; private set; }

    internal VkRenderPass  RenderPass => this.renderPass.Resource;

    public DepthStencilAttachment? DepthStencilAttachment { get; private set; }

    public IReadOnlyList<ColorAttachment> ColorAttachments => this.colorAttachments;

    public Size<uint> Size { get; }

    public RenderTarget(in CreateInfo createInfo)
    {
        var attachments = new MultiPassCreateInfo.AttachmentInfo[createInfo.ColorAttachments.Length + (createInfo.DepthStencilAttachment.HasValue ? 1 : 0)];

        var colorAttachments = new int[createInfo.ColorAttachments.Length];

        for (var i = 0; i < createInfo.ColorAttachments.Length; i++)
        {
            attachments[i]      = createInfo.ColorAttachments[i];
            colorAttachments[i] = i;
        }

        int? depthStencilAttachment = null;

        if (createInfo.DepthStencilAttachment.HasValue)
        {
            attachments[^1] = createInfo.DepthStencilAttachment.Value;

            depthStencilAttachment = attachments.Length - 1;
        }

        var multiPassCreateInfo = new MultiPassCreateInfo
        {
            Size        = createInfo.Size,
            Attachments = attachments,
            Passes      =
            [
                new()
                {
                    ColorAttachments       = colorAttachments,
                    DepthStencilAttachment = depthStencilAttachment,
                }
            ]
        };

        this.Size = multiPassCreateInfo.Size;

        this.renderPass = CreateRenderPass(multiPassCreateInfo);

        this.CreateResources(this.renderPass.Resource, multiPassCreateInfo);
    }

    public RenderTarget(in MultiPassCreateInfo multiPassCreateInfo)
    {
        this.Size = multiPassCreateInfo.Size;

        this.renderPass = CreateRenderPass(multiPassCreateInfo);

        this.CreateResources(this.renderPass.Resource, multiPassCreateInfo);
    }

    [MemberNotNull(nameof(Framebuffer))]
    private void CreateResources(VkRenderPass renderPass, in MultiPassCreateInfo multiPassCreateInfo)
    {
        this.colorAttachments.EnsureCapacity(multiPassCreateInfo.Attachments.Length);

        var imageViews = new List<VkImageView>(this.colorAttachments.Count);

        for (var i = 0; i < multiPassCreateInfo.Attachments.Length; i++)
        {
            ref readonly var attachment = ref multiPassCreateInfo.Attachments[i];

            if (attachment.TryGetColorAttachment(out var colorAttachment))
            {
                Texture2D? colorTexture;
                Texture2D? resolveTexture = null;

                if (colorAttachment.Image != null)
                {
                    colorTexture = new Texture2D(colorAttachment.Image, false, TextureAspect.Color);

                    imageViews.Add(colorTexture.ImageView);
                }
                else
                {
                    var imageCreateInfo = new VkImageCreateInfo
                    {
                        ArrayLayers   = 1,
                        Extent        = multiPassCreateInfo.Size.ToExtent3D(),
                        Format        = (VkFormat)colorAttachment.Format,
                        ImageType     = VkImageType.N2D,
                        InitialLayout = VkImageLayout.Undefined,
                        MipLevels     = 1,
                        Samples       = (VkSampleCountFlags)colorAttachment.SampleCount,
                        Tiling        = VkImageTiling.Optimal,
                        Usage         = (VkImageUsageFlags)colorAttachment.Usage | VkImageUsageFlags.TransferDst,
                    };

                    var colorImage = new Image(imageCreateInfo);
                    colorImage.ClearColor(Color.Margenta, (VkImageLayout)colorAttachment.FinalLayout);

                    colorTexture = new Texture2D(colorImage, true, TextureAspect.Color);

                    imageViews.Add(colorTexture.ImageView);

                    if (colorAttachment.EnableResolve)
                    {
                        imageCreateInfo.Samples = VkSampleCountFlags.N1;

                        resolveTexture = new(new Image(imageCreateInfo), true, colorTexture.Aspect);

                        imageViews.Add(resolveTexture.ImageView);
                    }
                }

                this.colorAttachments.Add(new(colorTexture, resolveTexture));
            }
            else if (attachment.TryGetDepthStencilAttachment(out var depthStencilAttachment))
            {
                if (this.DepthStencilAttachment.HasValue)
                {
                    foreach (var item in this.colorAttachments)
                    {
                        item.Dispose();
                    }

                    this.colorAttachments.Clear();

                    throw new InvalidOperationException("RenderTarget may only have one depth/stencil attachment.");
                }

                if (depthStencilAttachment.Image != null)
                {
                    this.DepthStencilAttachment = new(new(depthStencilAttachment.Image, false, depthStencilAttachment.Aspect));
                }
                else
                {
                    var imageCreateInfo = new VkImageCreateInfo
                    {
                        ArrayLayers   = 1,
                        Extent        = multiPassCreateInfo.Size.ToExtent3D(),
                        Format        = (VkFormat)depthStencilAttachment.Format,
                        ImageType     = VkImageType.N2D,
                        InitialLayout = VkImageLayout.Undefined,
                        MipLevels     = 1,
                        Samples       = VkSampleCountFlags.N1,
                        Tiling        = VkImageTiling.Optimal,
                        Usage         = VkImageUsageFlags.DepthStencilAttachment | (VkImageUsageFlags)depthStencilAttachment.Usage,
                    };

                    var image = new Image(imageCreateInfo);

                    this.DepthStencilAttachment = new(new(image, true, depthStencilAttachment.Aspect));
                }

                imageViews.Add(this.DepthStencilAttachment.Value.Texture.ImageView);
            }
        }

        this.Framebuffer = VulkanRenderer.Singleton.Context.CreateFrameBuffer(renderPass, imageViews.AsSpan(), multiPassCreateInfo.Size.ToExtent2D());
    }

    private unsafe static SharedResource<VkRenderPass> CreateRenderPass(in MultiPassCreateInfo createInfo)
    {
        var hashcode = createInfo.GetHashCode();

        ref var renderPass = ref renderPasses.GetValueRefOrAddDefault(hashcode, out var exists);

        if (!exists || renderPass!.IsDisposed)
        {
            using var disposables = new Disposables();

            using var subpassDescriptions     = new RefList<VkSubpassDescription>();
            using var attachmentDescriptions  = new RefList<VkAttachmentDescription>();
            using var depthStencilAttachments = new RefList<VkAttachmentDescription>();

            for (var i = 0; i < createInfo.Passes.Length; i++)
            {
                ref readonly var pass = ref createInfo.Passes[i];

                var colorAttachmentReferences        = new NativeList<VkAttachmentReference>(pass.ColorAttachments.Length);
                var resolveAttachmentReferences      = new NativeList<VkAttachmentReference>();
                var depthStencilAttachmentReferences = new NativeList<VkAttachmentReference>();

                disposables.Add(colorAttachmentReferences);
                disposables.Add(resolveAttachmentReferences);
                disposables.Add(depthStencilAttachmentReferences);

                for (var j = 0; j < pass.ColorAttachments.Length; j++)
                {
                    var index = pass.ColorAttachments[j];

                    if (!createInfo.Attachments[index].TryGetColorAttachment(out var colorAttachment))
                    {
                        throw new InvalidOperationException($"Passes[{i}].ColorAttachments[{j}] must reference a valid {nameof(CreateInfo.ColorAttachmentInfo)} at {index}, but got {createInfo.Attachments[index]}");
                    }

                    colorAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.ColorAttachmentOptimal });

                    var colorAttachmentDescription = new VkAttachmentDescription
                    {
                        Format         = (VkFormat)colorAttachment.Format,
                        Samples        = (VkSampleCountFlags)colorAttachment.SampleCount,
                        FinalLayout    = (VkImageLayout)colorAttachment.FinalLayout,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.Store,
                        StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare,
                    };

                    attachmentDescriptions.Add(colorAttachmentDescription);

                    if (!colorAttachment.EnableResolve)
                    {
                        resolveAttachmentReferences.Add(new() { Attachment = VkConstants.VK_ATTACHMENT_UNUSED, Layout = VkImageLayout.Undefined });
                    }
                    else
                    {
                        var resolveAttachmentDescription = colorAttachmentDescription;

                        resolveAttachmentDescription.Samples = VkSampleCountFlags.N1;
                        resolveAttachmentDescription.LoadOp  = VkAttachmentLoadOp.DontCare;

                        resolveAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.ColorAttachmentOptimal });
                        attachmentDescriptions.Add(resolveAttachmentDescription);
                    }
                }

                if (pass.DepthStencilAttachment is int depthStencilAttachmentIndex)
                {
                    if (!createInfo.Attachments[depthStencilAttachmentIndex].TryGetDepthStencilAttachment(out var depthStencilAttachmentInfo))
                    {
                        throw new InvalidOperationException($"Passes[{i}].DepthStencilAttachment must reference a valid {nameof(CreateInfo.DepthStencilAttachmentInfo)} at {depthStencilAttachmentIndex}, but got {createInfo.Attachments[depthStencilAttachmentIndex]}");
                    }

                    depthStencilAttachmentReferences.Add(new() { Attachment = (uint)attachmentDescriptions.Count, Layout = VkImageLayout.DepthStencilAttachmentOptimal });

                    var depthStencilAttachmentDescription = new VkAttachmentDescription
                    {
                        Format         = (VkFormat)depthStencilAttachmentInfo.Format,
                        FinalLayout    = (VkImageLayout)depthStencilAttachmentInfo.FinalLayout,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        Samples        = VkSampleCountFlags.N1,
                        StencilLoadOp  = VkAttachmentLoadOp.Clear,
                        StencilStoreOp = VkAttachmentStoreOp.DontCare,
                        StoreOp        = VkAttachmentStoreOp.Store,
                    };

                    attachmentDescriptions.Add(depthStencilAttachmentDescription);
                }

                var subpassDescription = new VkSubpassDescription
                {
                    PipelineBindPoint       = VkPipelineBindPoint.Graphics,
                    PColorAttachments       = colorAttachmentReferences.AsPointer(),
                    PResolveAttachments     = resolveAttachmentReferences.AsPointer(),
                    ColorAttachmentCount    = (uint)colorAttachmentReferences.Count,
                    PDepthStencilAttachment = depthStencilAttachmentReferences.AsPointer(),
                };

                subpassDescriptions.Add(subpassDescription);
            }

            var renderPassCreateInfo = new VkRenderPassCreateInfo
            {
                AttachmentCount = (uint)attachmentDescriptions.Count,
                PAttachments    = attachmentDescriptions.AsPointer(),
                SubpassCount    = (uint)subpassDescriptions.Count,
                PSubpasses      = subpassDescriptions.AsPointer(),
            };

            renderPass = new(VulkanRenderer.Singleton.Context.Device.CreateRenderPass(renderPassCreateInfo));
        }

        return renderPass!.Share();
    }

    protected override void OnDisposed()
    {
        VulkanRenderer.Singleton.DeferredDispose(this.renderPass);
        VulkanRenderer.Singleton.DeferredDispose(this.Framebuffer);

        foreach (var attachment in this.colorAttachments)
        {
            attachment.Dispose();
        }

        this.colorAttachments.Clear();

        this.DepthStencilAttachment?.Dispose();
        this.DepthStencilAttachment = null;
    }
}
