using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Uniforms;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.RenderPasses;

internal class CanvasRenderGraphPass : CanvasBaseRenderGraphPass
{
    private readonly Framebuffer[]                   framebuffers = new Framebuffer[VulkanContext.MAX_FRAMES_IN_FLIGHT];
    private readonly IndexBuffer                     indexBuffer;
    private readonly RenderPass                      renderPass;
    private readonly Dictionary<Texture, UniformSet> uniformSets = [];
    private readonly VertexBuffer                    vertexBuffer;
    private readonly IndexBuffer                     wireframeIndexBuffer;

    private UniformSet? lastUniformSet;

    protected override Color             ClearColor    { get; } = Color.White;
    protected override CommandBuffer     CommandBuffer => this.Renderer.CurrentCommandBuffer;
    protected override Framebuffer       Framebuffer   => this.framebuffers[this.Window.Surface.CurrentBuffer];
    protected override RenderResources[] Resources     { get; } = [];

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

        var canvasPipeline          = renderer.CreatePipelineAndWatch<CanvasShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);
        var canvasWireframePipeline = renderer.CreatePipelineAndWatch<CanvasWireframeShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);

        this.Resources =
        [
            new(canvasPipeline,          this.vertexBuffer, this.indexBuffer, true),
            new(canvasWireframePipeline, this.vertexBuffer, this.wireframeIndexBuffer, false),
        ];

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
                                StencilStoreOp = VkAttachmentStoreOp.DontCare
                            },
                        }
                    ],
                }
            ]
        };

        return this.Renderer.CreateRenderPass(createInfo);
    }

    private void CreateFrameBuffers()
    {
        var extent = new VkExtent3D
        {
            Width  = this.Window.Surface.Swapchain.Extent.Width,
            Height = this.Window.Surface.Swapchain.Extent.Height,
            Depth  = 1,
        };

        for (var i = 0; i < this.Window.Surface.Swapchain.Images.Length; i++)
        {
            var createInfo = new FramebufferCreateInfo
            {
                RenderPass  = this.renderPass,
                Attachments =
                [
                    new FramebufferCreateInfo.Attachment
                    {
                        Image = new(
                            this.Window.Surface.Swapchain.Images[i],
                            extent,
                            this.Window.Surface.Swapchain.Format,
                            VkImageType.N2D,
                            this.Window.Surface.Swapchain.ImageUsage,
                            VkImageLayout.PresentSrcKHR
                        ),
                        ImageAspect = VkImageAspectFlags.Color,
                    },
                ]
            };

            this.framebuffers[i] = this.Renderer.CreateFramebuffer(createInfo);
        }
    }

    private void DisposeFrameBuffers()
    {
        for (var i = 0; i < this.framebuffers.Length; i++)
        {
            this.framebuffers[i].Dispose();
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
            UV        = command.SampledTexture.UV,
            Viewport  = viewport,
        };

        if (!this.uniformSets.TryGetValue(command.SampledTexture.Texture, out var uniformSet))
        {
            var combinedImageSamplerUniform = new CombinedImageSamplerUniform
            {
                Binding     = 0,
                Sampler     = command.SampledTexture.Sampler,
                Texture     = command.SampledTexture.Texture,
                ImageLayout = VkImageLayout.ShaderReadOnlyOptimal,
            };

            this.uniformSets[command.SampledTexture.Texture] = uniformSet = new UniformSet(resource.Pipeline, [combinedImageSamplerUniform]);
        }

        if (uniformSet != null && uniformSet != this.lastUniformSet)
        {
            this.CommandBuffer.BindUniformSet(uniformSet);

            this.lastUniformSet = uniformSet;
        }

        this.CommandBuffer.PushConstant(resource.Pipeline, constant);
        this.CommandBuffer.DrawIndexed(resource.IndexBuffer);
    }

    protected override void OnDispose()
    {
        this.DisposeFrameBuffers();

        for (var i = 0; i < this.Resources.Length; i++)
        {
            this.Resources[i].Dispose();
        }

        foreach (var uniformSet in this.uniformSets.Values)
        {
            uniformSet.Dispose();
        }

        this.renderPass.Dispose();
    }

    public override void Recreate()
    {
        this.DisposeFrameBuffers();
        this.CreateFrameBuffers();
    }
}
