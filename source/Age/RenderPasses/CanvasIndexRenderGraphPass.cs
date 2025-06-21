using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Rendering.Vulkan;
using Age.Services;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using Age.Core.Extensions;

namespace Age.RenderPasses;

public sealed class CanvasIndexRenderGraphPass : CanvasBaseRenderGraphPass
{
    private readonly CanvasStencilMaskShader canvasStencilMaskShader;
    private readonly CommandBuffer           commandBuffer;
    private readonly IndexBuffer             indexBuffer;
    private readonly RenderPass              renderPass;
    private readonly VertexBuffer            vertexBuffer;

    private Image       colorImage;
    private Image       stencilImage;
    private Framebuffer framebuffer;

    public Image ColorImage => this.colorImage;

    protected override CanvasStencilMaskShader CanvasStencilMaskShader => this.canvasStencilMaskShader;
    protected override Color                   ClearColor              { get; } = Color.Black;
    protected override CommandBuffer           CommandBuffer           => this.commandBuffer;
    protected override Framebuffer             Framebuffer             => this.framebuffer;
    protected override RenderPipelines[]       Pipelines               { get; } = [];
    protected override PipelineVariant         PipelineVariants        { get; } = PipelineVariant.Index;

    public override RenderPass RenderPass => this.renderPass;

    public CanvasIndexRenderGraphPass(VulkanRenderer renderer, Window window) : base(renderer, window)
    {
        var vertices = new CanvasShader.Vertex[4]
        {
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        };

        this.vertexBuffer  = renderer.CreateVertexBuffer(vertices.AsSpan());
        this.indexBuffer   = renderer.CreateIndexBuffer([0u, 1, 2, 0, 2, 3]);
        this.commandBuffer = renderer.AllocateCommand(VkCommandBufferLevel.Primary);
        this.renderPass    = this.CreateRenderPass();

        this.CreateFramebuffer(out this.colorImage, out this.stencilImage, out this.framebuffer);

        this.canvasStencilMaskShader = new CanvasStencilMaskShader(this.renderPass, true);

        var shader = new CanvasIndexShader(this.renderPass, true);

        this.Pipelines =
        [
            new RenderPipelines(shader, this.vertexBuffer, this.indexBuffer, true, false)
        ];

        this.canvasStencilMaskShader.Changed += RenderingService.Singleton.RequestDraw;
        shader.Changed += RenderingService.Singleton.RequestDraw;
    }

    private void CreateFramebuffer(out Image colorImage, out Image stencilImage, out Framebuffer framebuffer)
    {
        var extent = new VkExtent3D
        {
            Width  = this.Window.Surface.Swapchain.Extent.Width,
            Height = this.Window.Surface.Swapchain.Extent.Height,
            Depth  = 1,
        };

        var colorImageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = extent,
            Format        = VkFormat.R16G16B16A16Unorm,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = VkSampleCountFlags.N1,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransferDst | VkImageUsageFlags.TransferSrc | VkImageUsageFlags.Sampled | VkImageUsageFlags.ColorAttachment,
        };

        colorImage = new Image(colorImageCreateInfo);

        colorImage.ClearColor(Color.Black, VkImageLayout.General);

        var stencilImageCreateInfo = colorImageCreateInfo;

        stencilImageCreateInfo.Format = VulkanRenderer.Singleton.StencilBufferFormat;
        stencilImageCreateInfo.Usage  = VkImageUsageFlags.DepthStencilAttachment;

        stencilImage = new Image(stencilImageCreateInfo);

        var framebufferCreateInfo = new FramebufferCreateInfo
        {
            RenderPass  = this.renderPass,
            Attachments =
            [
                new FramebufferCreateInfo.Attachment
                {
                    Image       = colorImage,
                    ImageAspect = VkImageAspectFlags.Color
                },
                new FramebufferCreateInfo.Attachment
                {
                    Image       = stencilImage,
                    ImageAspect = VkImageAspectFlags.Stencil
                },
            ],
        };

        framebuffer = new(framebufferCreateInfo);
    }

    private RenderPass CreateRenderPass()
    {
        var createInfo = new RenderPassCreateInfo
        {
            SubPasses =
            [
                new()
                {
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    ColorAttachments  =
                    [
                        new()
                        {
                            Color  = new VkAttachmentDescription
                            {
                                Format         = VkFormat.R16G16B16A16Unorm,
                                Samples        = VkSampleCountFlags.N1,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = VkImageLayout.General,
                                LoadOp         = VkAttachmentLoadOp.Clear,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare
                            },
                        }
                    ],
                    DepthStencilAttachment = new()
                    {
                        Format         = VulkanRenderer.Singleton.StencilBufferFormat,
                        Samples        = VkSampleCountFlags.N1,
                        InitialLayout  = VkImageLayout.Undefined,
                        FinalLayout    = VkImageLayout.DepthStencilAttachmentOptimal,
                        LoadOp         = VkAttachmentLoadOp.Clear,
                        StoreOp        = VkAttachmentStoreOp.DontCare,
                        StencilLoadOp  = VkAttachmentLoadOp.Clear,
                        StencilStoreOp = VkAttachmentStoreOp.Store
                    },
                },
            ],
            SubpassDependencies =
            [
                new()
                {
                    SrcSubpass    = VkConstants.VK_SUBPASS_EXTERNAL,
                    DstSubpass    = 0,
                    SrcStageMask  = VkPipelineStageFlags.FragmentShader,
                    DstStageMask  = VkPipelineStageFlags.EarlyFragmentTests | VkPipelineStageFlags.LateFragmentTests,
                    SrcAccessMask = VkAccessFlags.ShaderRead,
                    DstAccessMask = VkAccessFlags.DepthStencilAttachmentWrite,
                }
            ]
        };

        return new(createInfo);
    }

    private void DisposeFramebuffer()
    {
        this.colorImage.Dispose();
        this.stencilImage.Dispose();
        this.framebuffer.Dispose();
    }

    protected unsafe override void AfterExecute()
    {
        this.commandBuffer.End();

        var commandBufferHandle = this.commandBuffer.Instance.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        this.Renderer.GraphicsQueue.Submit(submitInfo);
        this.Renderer.GraphicsQueue.WaitIdle();
    }

    protected override void BeforeExecute()
    {
        this.commandBuffer.Reset();
        this.commandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);
    }

    protected override void ExecuteCommand(RenderPipelines resource, RectCommand command, in Size<float> viewport, in Transform2D transform)
    {
        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = 0xFFFF_0000_0000_0000 | command.ObjectId,
            Flags     = command.Flags,
            Size      = command.Size,
            Transform = command.Transform * transform,
            UV        = command.MappedTexture.UV,
            Viewport  = viewport,
        };

        this.CommandBuffer.PushConstant(resource.Shader, constant);
        this.CommandBuffer.DrawIndexed(resource.IndexBuffer);
    }

    protected override void Disposed()
    {
        this.DisposeFramebuffer();
        this.canvasStencilMaskShader.Dispose();
        this.renderPass.Dispose();
        this.commandBuffer.Dispose();
        this.Pipelines[0].Dispose();
    }

    public override void Recreate()
    {
        this.DisposeFramebuffer();
        this.CreateFramebuffer(out this.colorImage, out this.stencilImage, out this.framebuffer);
    }
}
