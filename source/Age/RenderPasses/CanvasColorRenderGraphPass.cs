using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Shaders;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using Age.Services;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using Age.Core.Extensions;
using Age.Graphs;

namespace Age.RenderPasses;

public sealed class CanvasColorRenderGraphPass : CanvasRenderGraphPass
{
    private readonly Image[]                           colorImages  = new Image[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly Framebuffer[]                     framebuffers = new Framebuffer[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly IndexBuffer32                     indexBuffer;
    private readonly Image[]                           stencilImages = new Image[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly VertexBuffer<CanvasShader.Vertex> vertexBuffer;
    private readonly IndexBuffer32                     wireframeIndexBuffer;

    protected override CanvasStencilMaskShader CanvasStencilEraserShader { get; }
    protected override CanvasStencilMaskShader CanvasStencilWriterShader { get; }
    protected override Color                   ClearColor                { get; } = Color.Black;
    protected override CommandBuffer           CommandBuffer             => this.Renderer.CurrentCommandBuffer;
    protected override CommandFilter           CommandFilter             { get; } = CommandFilter.Color;
    protected override Framebuffer             Framebuffer               => this.framebuffers[this.Window.Surface.CurrentBuffer];
    protected override RenderPipelines[]       Pipelines                 { get; } = [];

    public override RenderPass RenderPass { get; }

    public CanvasColorRenderGraphPass(VulkanRenderer renderer, Window window) : base(renderer, window)
    {
        var vertices = new CanvasShader.Vertex[4]
        {
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        };

        this.vertexBuffer         = new(vertices.AsSpan());
        this.indexBuffer          = new([0u, 1, 2, 0, 2, 3]);
        this.wireframeIndexBuffer = new([0u, 1, 1, 2, 2, 3, 3, 0, 0, 2]);

        this.RenderPass = CreateRenderPass(this.Window.Surface.Swapchain.Format, VkImageLayout.PresentSrcKHR);

        this.CanvasStencilWriterShader = new CanvasStencilMaskShader(this.RenderPass, StencilOp.Write, true);
        this.CanvasStencilEraserShader = new CanvasStencilMaskShader(this.RenderPass, StencilOp.Erase, true);

        var canvasShader          = new CanvasShader(this.RenderPass, true);
        var canvasWireframeShader = new CanvasWireframeShader(this.RenderPass, true);

        this.Pipelines =
        [
            new(canvasShader,          this.vertexBuffer, this.indexBuffer, true, false),
            new(canvasWireframeShader, this.vertexBuffer, this.wireframeIndexBuffer, false, true),
        ];

        this.CanvasStencilWriterShader.Changed += RenderingService.Singleton.RequestDraw;
        this.CanvasStencilEraserShader.Changed += RenderingService.Singleton.RequestDraw;

        canvasShader.Changed          += RenderingService.Singleton.RequestDraw;
        canvasWireframeShader.Changed += RenderingService.Singleton.RequestDraw;

        var extent = new VkExtent3D
        {
            Width  = this.Window.Surface.Swapchain.Extent.Width,
            Height = this.Window.Surface.Swapchain.Extent.Height,
            Depth  = 1,
        };

        this.CreateFrameBuffers();
    }

    private void CreateFrameBuffers()
    {
        var extent = new VkExtent3D
        {
            Width  = this.Window.Surface.Swapchain.Extent.Width,
            Height = this.Window.Surface.Swapchain.Extent.Height,
            Depth  = 1,
        };

        var stencilImageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = extent,
            Format        = VulkanRenderer.Singleton.StencilBufferFormat,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = VkSampleCountFlags.N1,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.DepthStencilAttachment,
        };

        for (var i = 0; i < this.Window.Surface.Swapchain.Images.Length; i++)
        {
            this.stencilImages[i] = new Image(stencilImageCreateInfo);
            this.colorImages[i]   = this.Window.Surface.Swapchain.Images[i];

            var createInfo = new FramebufferCreateInfo
            {
                RenderPass  = this.RenderPass,
                Attachments =
                [
                    new FramebufferCreateInfo.Attachment
                    {
                        Image       = this.colorImages[i],
                        ImageAspect = VkImageAspectFlags.Color,
                    },
                    new FramebufferCreateInfo.Attachment
                    {
                        Image       = this.stencilImages[i],
                        ImageAspect = VkImageAspectFlags.Stencil,
                    },
                ]
            };

            this.framebuffers[i] = new(createInfo);
        }
    }

    private void DisposeFrameBuffers()
    {
        for (var i = 0; i < VulkanContext.MAX_FRAMES_IN_FLIGHT; i++)
        {
            VulkanRenderer.Singleton.DeferredDispose(this.stencilImages[i]);
            VulkanRenderer.Singleton.DeferredDispose(this.framebuffers[i]);
        }
    }

    protected override void BeforeExecute() =>
        this.LastUniformSet = null;

    protected override ReadOnlySpan<Command2D> GetCommand(RenderContext renderContext) =>
        renderContext.Buffer2D.Commands;

    protected override void Disposed()
    {
        this.DisposeFrameBuffers();

        for (var i = 0; i < this.Pipelines.Length; i++)
        {
            this.Pipelines[i].Dispose();
            this.Pipelines[i].Shader.Changed -= RenderingService.Singleton.RequestDraw;
        }

        this.RenderPass.Dispose();
        this.CanvasStencilWriterShader.Dispose();
        this.CanvasStencilEraserShader.Dispose();
        this.RenderPass.Dispose();
    }

    protected override void ExecuteCommand(RenderPipelines resource, RectCommand command, in Size<uint> viewport)
    {
        if (!this.UniformSets.TryGetValue(command.TextureMap.Texture, out var uniformSet))
        {
            var diffuse = new CombinedImageSamplerUniform
            {
                Binding     = 0,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                Image       = command.TextureMap.Texture.Image,
                ImageView   = command.TextureMap.Texture.ImageView,
                Sampler     = command.TextureMap.Texture.Sampler,
            };

            this.UniformSets.Set(command.TextureMap.Texture, uniformSet = new UniformSet(resource.Shader, [diffuse]));
        }

        if (uniformSet != null && this.LastUniformSet != uniformSet)
        {
            this.CommandBuffer.BindUniformSet(this.LastUniformSet = uniformSet);
        }

        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = command.Color,
            Flags     = command.Flags,
            Size      = command.Size,
            Transform = command.Matrix,
            UV        = command.TextureMap.UV,
            Viewport  = viewport,
        };

        this.CommandBuffer.PushConstant(resource.Shader, constant);
        this.CommandBuffer.DrawIndexed(resource.IndexBuffer);
    }

    public override void Recreate()
    {
        this.DisposeFrameBuffers();
        this.CreateFrameBuffers();
    }
}
