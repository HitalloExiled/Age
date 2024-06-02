using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.RenderPasses;

public class CanvasIdRenderGraphPass : CanvasBaseRenderGraphPass
{
    private readonly VkCommandBuffer commandBuffer;
    private readonly IndexBuffer     indexBuffer;
    private readonly VertexBuffer    vertexBuffer;

    private RenderPass renderPass;
    public Image       Image;

    protected override VkCommandBuffer CommandBuffer  => this.commandBuffer;
    protected override RenderResources[] Resources       { get; } = [];
    protected override uint            CurrentBuffer  { get; }
    protected override RenderPass      RenderPass     => this.renderPass;

    public CanvasIdRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
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
        this.commandBuffer = renderer.Context.AllocateCommand(VkCommandBufferLevel.Primary);

        this.Create(out this.Image, out this.renderPass);

        var shader = renderer.CreateShaderAndWatch<CanvasObjectIdShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);

        shader.Changed += this.NotifyChanged;

        this.Resources =
        [
            new RenderResources(shader, this.vertexBuffer, this.indexBuffer)
        ];
    }

    private void Create(out Image image, out RenderPass renderPass)
    {
        var clientSize = this.Window.ClientSize;

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = new()
            {
                Width  = clientSize.Width,
                Height = clientSize.Height,
                Depth  = 1,
            },
            Format        = this.Window.Surface.Swapchain.Format,
            ImageType     = VkImageType.N2D,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = VkSampleCountFlags.N1,
            Tiling        = VkImageTiling.Optimal,
            Usage         = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.Sampled | VkImageUsageFlags.ColorAttachment,
        };

        image = this.Renderer.CreateImage(imageCreateInfo);

        var createInfo = new RenderPass.CreateInfo
        {
            FrameBufferCount = 1,
            Extent           = new()
            {
                Width  = clientSize.Width,
                Height = clientSize.Height,
            },
            SubPasses =
            [
                new()
                {
                    PipelineBindPoint = VkPipelineBindPoint.Graphics,
                    Images            = [this.Image.Value],
                    ImageAspect       = VkImageAspectFlags.Color,
                    Format            = this.Window.Surface.Swapchain.Format,
                    ColorAttachments  =
                    [
                        new()
                        {
                            Color  = new VkAttachmentDescription
                            {
                                Format         = this.Window.Surface.Swapchain.Format,
                                Samples        = VkSampleCountFlags.N1,
                                InitialLayout  = VkImageLayout.Undefined,
                                FinalLayout    = VkImageLayout.TransferSrcOptimal,
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

        renderPass = this.Renderer.CreateRenderPass(createInfo);
    }

    protected unsafe override void AfterExecute()
    {
        this.commandBuffer.End();

        var commandBufferHandle = this.commandBuffer.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        this.Renderer.Context.GraphicsQueue.Submit(submitInfo);
        this.Renderer.Context.GraphicsQueue.WaitIdle();
    }

    protected override void BeforeExecute()
    {
        this.commandBuffer.Reset();
        this.commandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);
    }

    protected override void ExecuteCommand(RenderResources resource, RectDrawCommand command, in Size<float> viewport, in Matrix3x2<float> transform)
    {
        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = command.ObjectId | 0b_11111111_00000000_00000000_00000000,
            Flags     = command.Flags,
            Rect      = command.Rect,
            Transform = transform,
            UV        = command.SampledTexture.UV,
            Viewport  = viewport,
        };

        this.Renderer.PushConstant(this.CommandBuffer, resource.Shader, constant);
        this.Renderer.DrawIndexed(this.CommandBuffer, resource.IndexBuffer);
    }

    protected override void OnDispose()
    {
        this.renderPass.Dispose();
        this.Image.Dispose();
        this.commandBuffer.Dispose();
        this.Resources[0].Dispose();
    }

    public override void Recreate()
    {
        this.renderPass.Dispose();
        this.Image.Dispose();

        this.Create(out this.Image, out this.renderPass);

        for (var i = 0; i < this.Resources.Length; i++)
        {
            this.Resources[i].Shader.RenderPass = this.renderPass;
        }
    }
}
