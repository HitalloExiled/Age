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
    private readonly IndexBuffer32                     indexBuffer;
    private readonly VertexBuffer<CanvasShader.Vertex> vertexBuffer;

    private Image       colorImage;
    private Framebuffer framebuffer;
    private Image       stencilImage;

    protected override CanvasStencilMaskShader CanvasStencilEraserShader { get; }
    protected override CanvasStencilMaskShader CanvasStencilWriterShader { get; }
    protected override Color                   ClearColor                { get; } = Color.Black;
    protected override CommandBuffer           CommandBuffer             { get; }
    protected override CommandFilter           CommandFilters            { get; } = CommandFilter.Index;
    protected override Framebuffer             Framebuffer               => this.framebuffer;
    protected override RenderPipelines[]       Pipelines                 { get; } = [];

    public override RenderPass RenderPass { get; }

    public Image ColorImage => this.colorImage;

    public CanvasIndexRenderGraphPass(VulkanRenderer renderer, Window window) : base(renderer, window)
    {
        var vertices = new CanvasShader.Vertex[4]
        {
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        };

        this.vertexBuffer  = new(vertices.AsSpan());
        this.indexBuffer   = new([0, 1, 2, 0, 2, 3]);
        this.CommandBuffer = renderer.AllocateCommand(VkCommandBufferLevel.Primary);
        this.RenderPass    = CreateRenderPass(VkFormat.R16G16B16A16Unorm, VkImageLayout.General);

        this.CreateFramebuffer(out this.colorImage, out this.stencilImage, out this.framebuffer);

        this.CanvasStencilWriterShader = new CanvasStencilMaskShader(this.RenderPass, StencilOp.Write, true);
        this.CanvasStencilEraserShader = new CanvasStencilMaskShader(this.RenderPass, StencilOp.Erase, true);

        var shader = new CanvasIndexShader(this.RenderPass, true);

        this.Pipelines =
        [
            new RenderPipelines(shader, this.vertexBuffer, this.indexBuffer, true, false)
        ];

        this.CanvasStencilWriterShader.Changed += RenderingService.Singleton.RequestDraw;
        this.CanvasStencilEraserShader.Changed += RenderingService.Singleton.RequestDraw;
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
            RenderPass  = this.RenderPass,
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

    private void DisposeFramebuffer()
    {
        this.colorImage.Dispose();
        this.stencilImage.Dispose();
        this.framebuffer.Dispose();
    }

    protected unsafe override void AfterExecute()
    {
        this.CommandBuffer.End();

        var commandBufferHandle = this.CommandBuffer.Instance.Handle;

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
        this.CommandBuffer.Reset();
        this.CommandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);
    }

    protected override void Disposed()
    {
        this.DisposeFramebuffer();
        this.CanvasStencilWriterShader.Dispose();
        this.CanvasStencilEraserShader.Dispose();
        this.RenderPass.Dispose();
        this.CommandBuffer.Dispose();
        this.Pipelines[0].Dispose();
    }

    protected override void ExecuteCommand(RenderPipelines resource, RectCommand command, in Size<uint> viewport)
    {
        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = 0xFFFF_0000_0000_0000 | command.ObjectId,
            Flags     = command.Flags,
            Size      = command.Size,
            Transform = command.Transform,
            UV        = UVRect.Normalized,
            Viewport  = viewport,
        };

        this.CommandBuffer.PushConstant(resource.Shader, constant);
        this.CommandBuffer.DrawIndexed(resource.IndexBuffer);
    }

    public override void Recreate()
    {
        this.DisposeFramebuffer();
        this.CreateFramebuffer(out this.colorImage, out this.stencilImage, out this.framebuffer);
    }
}
