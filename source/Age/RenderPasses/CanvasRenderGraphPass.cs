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

namespace Age.RenderPasses;

public sealed class CanvasRenderGraphPass : CanvasBaseRenderGraphPass
{
    private readonly CanvasStencilMaskShader canvasStencilMaskShader;
    private readonly Image[]                 colorImages = new Image[VulkanContext.MAX_FRAMES_IN_FLIGHT];
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
    protected override RenderPipelines[]       Pipelines               { get; } = [];

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

        var canvasShader          = new CanvasShader(this.renderPass, true);
        var canvasWireframeShader = new CanvasWireframeShader(this.renderPass, true);

        this.Pipelines =
        [
            new(canvasShader,          this.vertexBuffer, this.indexBuffer, true, false),
            new(canvasWireframeShader, this.vertexBuffer, this.wireframeIndexBuffer, false, true),
        ];

        this.canvasStencilMaskShader.Changed += RenderingService.Singleton.RequestDraw;
        canvasShader.Changed                 += RenderingService.Singleton.RequestDraw;
        canvasWireframeShader.Changed        += RenderingService.Singleton.RequestDraw;

        var extent = new VkExtent3D
        {
            Width  = this.Window.Surface.Swapchain.Extent.Width,
            Height = this.Window.Surface.Swapchain.Extent.Height,
            Depth  = 1,
        };

        this.CreateFrameBuffers();
    }

    protected override void BeforeExecute() =>
        this.lastUniformSet = null;

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
            this.colorImages[i]   = new Image(
                this.Window.Surface.Swapchain.Images[i],
                new()
                {
                    Extent        = extent,
                    Format        = this.Window.Surface.Swapchain.Format,
                    ImageType     = VkImageType.N2D,
                    Usage         = this.Window.Surface.Swapchain.ImageUsage,
                    InitialLayout = VkImageLayout.ColorAttachmentOptimal,
                }
            );

            var createInfo = new FramebufferCreateInfo
            {
                RenderPass  = this.renderPass,
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
                }
            ],
            // SubpassDependencies =
            // [
            //     new()
            //     {
            //         SrcSubpass    = VkConstants.VK_SUBPASS_EXTERNAL,
            //         DstSubpass    = 0,
            //         SrcStageMask  = VkPipelineStageFlags.FragmentShader,
            //         DstStageMask  = VkPipelineStageFlags.EarlyFragmentTests | VkPipelineStageFlags.LateFragmentTests,
            //         SrcAccessMask = VkAccessFlags.ShaderRead,
            //         DstAccessMask = VkAccessFlags.DepthStencilAttachmentWrite,
            //     }
            // ]
        };

        return new(createInfo);
    }

    private void DisposeFrameBuffers()
    {
        for (var i = 0; i < VulkanContext.MAX_FRAMES_IN_FLIGHT; i++)
        {
            VulkanRenderer.Singleton.DeferredDispose(this.stencilImages[i]);
            VulkanRenderer.Singleton.DeferredDispose(this.framebuffers[i]);
        }
    }

    protected override void ExecuteCommand(RenderPipelines resource, RectCommand command, in Size<float> viewport, in Transform2D transform)
    {
        if (command.PipelineVariant.HasAnyFlag(PipelineVariant.Color | PipelineVariant.Wireframe))
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

            if (!this.UniformSets.TryGetValue(command.MappedTexture.Texture, out var uniformSet))
            {
                var diffuse = new CombinedImageSamplerUniform
                {
                    Binding     = 0,
                    Texture     = command.MappedTexture.Texture,
                    ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
                };

                this.UniformSets.Set(command.MappedTexture.Texture, uniformSet = new UniformSet(resource.Shader, [diffuse]));
            }

            if (uniformSet != null && uniformSet != this.lastUniformSet)
            {
                this.CommandBuffer.BindUniformSet(uniformSet);

                this.lastUniformSet = uniformSet;
            }

            this.CommandBuffer.PushConstant(resource.Shader, constant);
            this.CommandBuffer.DrawIndexed(resource.IndexBuffer);
        }
    }

    protected override void Disposed()
    {
        this.DisposeFrameBuffers();
        for (var i = 0; i < this.Pipelines.Length; i++)
        {
            this.Pipelines[i].Dispose();
            this.Pipelines[i].Shader.Changed -= RenderingService.Singleton.RequestDraw;
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
