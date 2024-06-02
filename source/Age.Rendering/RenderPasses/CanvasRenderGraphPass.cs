using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Storage;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.RenderPasses;

internal class CanvasRenderGraphPass : CanvasBaseRenderGraphPass
{
    private readonly TextureStorage textureStorage;
    private readonly IndexBuffer    indexBuffer;
    private readonly IndexBuffer    wireframeIndexBuffer;
    private readonly VertexBuffer   vertexBuffer;

    private RenderPass  renderPass; // TODO implements renderPassReuse
    private UniformSet? lastUniformSet;

    protected override VkCommandBuffer CommandBuffer  => this.Renderer.Context.Frame.CommandBuffer;
    protected override RenderResources[] Resources       { get; } = [];
    protected override uint            CurrentBuffer  => this.Window.Surface.CurrentBuffer;
    protected override RenderPass      RenderPass     => this.renderPass;

    public CanvasRenderGraphPass(VulkanRenderer renderer, IWindow window, TextureStorage textureStorage) : base(renderer, window)
    {
        this.textureStorage = textureStorage;

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

        var canvasShader          = renderer.CreateShaderAndWatch<CanvasShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);
        var canvasWireframeShader = renderer.CreateShaderAndWatch<CanvasWireframeShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);

        canvasShader.Changed          += this.NotifyChanged;
        canvasWireframeShader.Changed += this.NotifyChanged;

        this.Resources =
        [
            new(canvasShader,          this.vertexBuffer, this.indexBuffer),
            new(canvasWireframeShader, this.vertexBuffer, this.wireframeIndexBuffer),
        ];
    }

    protected override void BeforeExecute() =>
        this.lastUniformSet = null;

    private RenderPass CreateRenderPass()
    {
        var createInfo = new RenderPass.CreateInfo
        {
            FrameBufferCount = this.Window.Surface.Swapchain.Images.Length,
            Extent           = this.Window.Surface.Swapchain.Extent,
            SubPasses =
            [
                new()
                {
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    Format            = this.Window.Surface.Swapchain.Format,
                    Images            = this.Window.Surface.Swapchain.Images,
                    ImageAspect       = VkImageAspectFlags.Color,
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

    protected override void ExecuteCommand(RenderResources resource, RectDrawCommand command, in Size<float> viewport, in Matrix3x2<float> transform)
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

        var uniformSet = this.textureStorage.GetUniformSet(resource.Shader, command.SampledTexture.Texture, command.SampledTexture.Sampler);

        if (uniformSet != null && uniformSet != this.lastUniformSet)
        {
            this.Renderer.BindUniformSet(this.CommandBuffer, uniformSet);

            this.lastUniformSet = uniformSet;
        }

        this.Renderer.PushConstant(this.CommandBuffer, resource.Shader, constant);
        this.Renderer.DrawIndexed(this.CommandBuffer,  resource.IndexBuffer);
    }

    protected override void OnDispose()
    {
        for (var i = 0; i < this.Resources.Length; i++)
        {
            this.Resources[i].Dispose();
        }

        this.renderPass.Dispose();
    }

    public override void Recreate()
    {
        this.renderPass.Dispose();

        this.renderPass = this.CreateRenderPass();

        for (var i = 0; i < this.Resources.Length; i++)
        {
            this.Resources[i].Shader.RenderPass = this.renderPass;
        }
    }
}
