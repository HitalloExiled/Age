using System.Dynamic;
using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Converters;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders.Canvas;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.RenderPasses;

public class CanvasIndexRenderGraphPass : CanvasBaseRenderGraphPass
{
    private readonly CommandBuffer commandBuffer;
    private readonly IndexBuffer   indexBuffer;
    private readonly RenderPass    renderPass;
    private readonly VertexBuffer  vertexBuffer;

    private Image       image;
    private Framebuffer framebuffer;

    public Image ColorImage => this.image;

    protected override Color             ClearColor    { get; } = Color.Black;
    protected override CommandBuffer     CommandBuffer => this.commandBuffer;
    protected override Framebuffer       Framebuffer   => this.framebuffer;
    protected override RenderResources[] Resources     { get; } = [];

    public override RenderPass RenderPass => this.renderPass;

    public CanvasIndexRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
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

        this.CreateFramebuffer(out this.image, out this.framebuffer);

        var shader = renderer.CreatePipelineAndWatch<CanvasIndexShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);

        this.Resources =
        [
            new RenderResources(shader, this.vertexBuffer, this.indexBuffer, true)
        ];
    }



    private void CreateFramebuffer(out Image image, out Framebuffer framebuffer)
    {
        var clientSize = this.Window.ClientSize;

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers = 1,
            Extent      = new()
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
            Usage         = VkImageUsageFlags.TransferDst | VkImageUsageFlags.TransferSrc | VkImageUsageFlags.Sampled | VkImageUsageFlags.ColorAttachment,
        };

        image = this.Renderer.CreateImage(imageCreateInfo);

        Span<byte> pixels = new byte[(int)(clientSize.Width * clientSize.Height * 4)];

        image.Update(pixels);

        var framebufferCreateInfo = new FramebufferCreateInfo
        {
            RenderPass  = this.renderPass,
            Attachments =
            [
                new FramebufferCreateInfo.Attachment
                {
                    Image       = image,
                    ImageAspect = VkImageAspectFlags.Color
                }
            ],
        };

        framebuffer = this.Renderer.CreateFramebuffer(framebufferCreateInfo);
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

        return this.Renderer.CreateRenderPass(createInfo);
    }

    private void DisposeFramebuffer()
    {
        this.image.Dispose();
        this.framebuffer.Dispose();
    }

    protected unsafe override void AfterExecute()
    {
        this.commandBuffer.End();

        var commandBufferHandle = this.commandBuffer.Value.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        this.Renderer.GraphicsQueue.Submit(submitInfo);
        this.Renderer.GraphicsQueue.WaitIdle();
        this.ColorImage.Initialized = true;
    }

    protected override void BeforeExecute()
    {
        this.commandBuffer.Reset();
        this.commandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);
    }

    protected override void ExecuteCommand(RenderResources resource, RectCommand command, in Size<float> viewport, in Matrix3x2<float> transform)
    {
        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = ColorFormatConverter.RGBAtoBGRA(command.ObjectId | 0xff000000),
            Flags     = command.Flags,
            Rect      = command.Rect,
            Transform = transform,
            UV        = command.SampledTexture.UV,
            Viewport  = viewport,
        };

        this.CommandBuffer.PushConstant(resource.Pipeline, constant);
        this.CommandBuffer.DrawIndexed(resource.IndexBuffer);
    }

    protected override void OnDispose()
    {
        this.DisposeFramebuffer();

        this.renderPass.Dispose();
        this.commandBuffer.Dispose();
        this.Resources[0].Dispose();
    }

    public override void Recreate()
    {
        this.DisposeFramebuffer();
        this.CreateFramebuffer(out this.image, out this.framebuffer);
    }
}
