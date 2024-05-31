using System.Diagnostics.CodeAnalysis;
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
    private readonly Shader          shader;

    private RenderPass renderPass;

    public Image       Image;

    protected override VkCommandBuffer CommandBuffer => this.commandBuffer;
    protected override uint            CurrentBuffer { get; }
    protected override Shader          Shader        => this.shader;

    public CanvasIdRenderGraphPass(VulkanRenderer renderer, IWindow window) : base(renderer, window)
    {
        this.commandBuffer = renderer.Context.AllocateCommand(VkCommandBufferLevel.Primary);

        this.Create();

        this.shader = renderer.CreateShaderAndWatch<CanvasObjectIdShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);
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

    [MemberNotNull(nameof(Image), nameof(renderPass))]
    protected override void Create()
    {
        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = new()
            {
                Width  = this.Window.Surface.Swapchain.Extent.Width,
                Height = this.Window.Surface.Swapchain.Extent.Height,
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

        this.Image = this.Renderer.CreateImage(imageCreateInfo);

        var createInfo = new RenderPass.CreateInfo
        {
            FrameBufferCount = 1,
            Extent           = this.Window.Surface.Swapchain.Extent,
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
                            Layout = VkImageLayout.ColorAttachmentOptimal,
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

        this.renderPass = this.Renderer.CreateRenderPass(createInfo);

        if (this.Shader != null)
        {
            this.Shader.RenderPass = this.renderPass;
        }
    }

    protected override void Destroy()
    {
        this.renderPass.Dispose();
        this.Image.Dispose();
    }

    protected override void OnDispose()
    {
        this.commandBuffer.Dispose();
        this.shader.Dispose();
        base.OnDispose();
    }

    protected override void ExecuteCommand(RectDrawCommand command, in Size<float> viewport, in Matrix3x2<float> transform)
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

        this.Renderer.PushConstant(this.CommandBuffer, this.shader, constant);
        this.Renderer.DrawIndexed(this.CommandBuffer, this.IndexBuffer);
    }
}
