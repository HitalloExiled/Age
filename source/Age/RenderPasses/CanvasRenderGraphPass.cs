using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using Age.Services;
using Age.Storage;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.RenderPasses;

internal class CanvasRenderGraphPass : CanvasBaseRenderGraphPass
{
    private readonly CanvasStencilMaskShader canvasStencilMaskShader;
    private readonly Framebuffer[]           framebuffers  = new Framebuffer[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly IndexBuffer             indexBuffer;
    private readonly RenderPass              renderPass;
    private readonly Image[]                 stencilImages = new Image[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly VertexBuffer            vertexBuffer;
    private readonly IndexBuffer             wireframeIndexBuffer;

    private UniformSet? lastUniformSet;

    protected override CanvasStencilMaskShader CanvasStencilMaskShader => this.canvasStencilMaskShader;
    protected override Color                   ClearColor              { get; } = Color.Black;
    protected override CommandBuffer           CommandBuffer           => this.Renderer.CurrentCommandBuffer;
    protected override Framebuffer             Framebuffer             => this.framebuffers[this.Window.Surface.CurrentBuffer];
    protected override RenderResources[]       Resources               { get; } = [];

    public override RenderPass RenderPass => this.renderPass;


    public CanvasRenderGraphPass(VulkanRenderer renderer, Window window) : base(renderer, window)
    {
        var vertices = new CanvasShader.Vertex[4]
        {
            new(-1, -1),
            new( 1, -1),
            new( 1,  1),
            new(-1,  1),
        };

        this.vertexBuffer         = renderer.CreateVertexBuffer(vertices.AsSpan());
        this.indexBuffer          = renderer.CreateIndexBuffer([0u, 1, 2, 0, 2, 3]);
        this.wireframeIndexBuffer = renderer.CreateIndexBuffer([0u, 1, 1, 2, 2, 3, 3, 0, 0, 2]);

        this.renderPass = this.CreateRenderPass();

        this.canvasStencilMaskShader = new CanvasStencilMaskShader(this.renderPass, true);

        var canvasPipeline          = new CanvasShader(this.renderPass, true);
        var canvasWireframePipeline = new CanvasWireframeShader(this.renderPass, true);

        this.Resources =
        [
            new(canvasPipeline,          this.vertexBuffer, this.indexBuffer, true),
            new(canvasWireframePipeline, this.vertexBuffer, this.wireframeIndexBuffer, true),
        ];

        this.canvasStencilMaskShader.Changed += RenderingService.Singleton.RequestDraw;
        canvasPipeline.Changed               += RenderingService.Singleton.RequestDraw;
        canvasWireframePipeline.Changed      += RenderingService.Singleton.RequestDraw;

        this.CreateFrameBuffers();
    }

    protected override void BeforeExecute() =>
        this.lastUniformSet = null;

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
                                Format         = this.Window.Surface.Swapchain.Format,
                                Samples        = VkSampleCountFlags.N1,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = VkImageLayout.PresentSrcKHR,
                                LoadOp         = VkAttachmentLoadOp.Clear,
                                StoreOp        = VkAttachmentStoreOp.Store,
                                StencilLoadOp  = VkAttachmentLoadOp.DontCare,
                                StencilStoreOp = VkAttachmentStoreOp.DontCare,
                            },
                        }
                    ],
                    // DepthStencilAttachment = new()
                    // {
                    //     Format         = VulkanRenderer.Singleton.StencilBufferFormat,
                    //     Samples        = VkSampleCountFlags.N1,
                    //     InitialLayout  = VkImageLayout.Undefined,
                    //     FinalLayout    = VkImageLayout.DepthStencilAttachmentOptimal,
                    //     LoadOp         = VkAttachmentLoadOp.Clear,
                    //     StoreOp        = VkAttachmentStoreOp.DontCare,
                    //     StencilLoadOp  = VkAttachmentLoadOp.Clear,
                    //     StencilStoreOp = VkAttachmentStoreOp.Store
                    // },
                }
            ],
            // SubpassDependencies =
            // [
            //     new()
            //     {
            //         SrcSubpass    = VkConstants.VK_SUBPASS_EXTERNAL,
            //         DstSubpass    = 0,
            //         SrcStageMask  = VkPipelineStageFlags.ColorAttachmentOutput,
            //         SrcAccessMask = 0,
            //         DstStageMask  = VkPipelineStageFlags.ColorAttachmentOutput,
            //         DstAccessMask = VkAccessFlags.ColorAttachmentWrite,
            //     }
            // ]
        };

        return new(createInfo);
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

            var createInfo = new FramebufferCreateInfo
            {
                RenderPass  = this.renderPass,
                Attachments =
                [
                    new FramebufferCreateInfo.Attachment
                    {
                        Image = new(
                            this.Window.Surface.Swapchain.Images[i],
                            new()
                            {
                                Extent        = extent,
                                Format        = this.Window.Surface.Swapchain.Format,
                                ImageType     = VkImageType.N2D,
                                Usage         = this.Window.Surface.Swapchain.ImageUsage,
                                InitialLayout = VkImageLayout.PresentSrcKHR,
                            }
                        ),
                        ImageAspect = VkImageAspectFlags.Color,
                    },
                    // new FramebufferCreateInfo.Attachment
                    // {
                    //     Image       = this.stencilImages[i],
                    //     ImageAspect = VkImageAspectFlags.Stencil,
                    // },
                ]
            };

            this.framebuffers[i] = this.Renderer.CreateFramebuffer(createInfo);
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

    protected override void ExecuteCommand(RenderResources resource, RectCommand command, in Size<float> viewport, in Matrix3x2<float> transform)
    {
        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = command.Color,
            Flags     = command.Flags,
            Rect      = command.Rect,
            Transform = transform,
            UV        = command.MappedTexture.UV,
            Viewport  = viewport,
        };

        var hashcode = command.MappedTexture.Texture.GetHashCode();

        if (!this.UniformSets.TryGetValue(hashcode, out var uniformSet))
        {
            var diffuse = new CombinedImageSamplerUniform
            {
                Binding     = 0,
                Texture     = command.MappedTexture.Texture,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
            };

            this.UniformSets[hashcode] = uniformSet = new UniformSet(resource.Shader, [diffuse]);
        }

        if (uniformSet != null && uniformSet != this.lastUniformSet)
        {
            this.CommandBuffer.BindUniformSet(uniformSet);

            this.lastUniformSet = uniformSet;
        }

        this.CommandBuffer.PushConstant(resource.Shader, constant);
        this.CommandBuffer.DrawIndexed(resource.IndexBuffer);
    }

    protected override void Disposed()
    {
        base.Disposed();

        this.DisposeFrameBuffers();

        for (var i = 0; i < this.Resources.Length; i++)
        {
            this.Resources[i].Dispose();
            this.Resources[i].Shader.Changed -= RenderingService.Singleton.RequestDraw;
        }

        this.canvasStencilMaskShader.Dispose();
        this.renderPass.Dispose();
    }

    public override void Recreate()
    {
        this.DisposeFrameBuffers();
        this.CreateFrameBuffers();
    }
}
