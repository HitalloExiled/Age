using System.Diagnostics.CodeAnalysis;
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
    private readonly Shader         shader;

    private RenderPass  renderPass;
    private UniformSet? lastUniformSet;

    protected override VkCommandBuffer CommandBuffer => this.Renderer.Context.Frame.CommandBuffer;
    protected override uint            CurrentBuffer => this.Window.Surface.CurrentBuffer;
    protected override Shader          Shader        => this.shader;

    public CanvasRenderGraphPass(VulkanRenderer renderer, IWindow window, TextureStorage textureStorage) : base(renderer, window)
    {
        this.textureStorage = textureStorage;

        this.Create();

        this.shader = renderer.CreateShaderAndWatch<CanvasShader, CanvasShader.Vertex, CanvasShader.PushConstant>(new(), this.renderPass);
    }

    protected override void BeforeExecute() =>
        this.lastUniformSet = null;

    [MemberNotNull(nameof(renderPass))]
    protected override void Create()
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
                            Layout = VkImageLayout.ColorAttachmentOptimal,
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

        this.renderPass = this.Renderer.CreateRenderPass(createInfo);

        if (this.Shader != null)
        {
            this.Shader.RenderPass = this.renderPass;
        }
    }

    protected override void Destroy() =>
        this.renderPass.Dispose();

    protected override void ExecuteCommand(RectDrawCommand command, in Size<float> viewport, in Matrix3x2<float> transform)
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

        var uniformSet = this.textureStorage.GetUniformSet(this.shader, command.SampledTexture.Texture, command.SampledTexture.Sampler);

        if (uniformSet != null && uniformSet != this.lastUniformSet)
        {
            this.Renderer.BindUniformSet(this.CommandBuffer, uniformSet);

            this.lastUniformSet = uniformSet;
        }

        this.Renderer.PushConstant(this.CommandBuffer, this.shader, constant);
        this.Renderer.DrawIndexed(this.CommandBuffer, this.IndexBuffer);
    }

    protected override void OnDispose()
    {
        this.shader.Dispose();
        base.OnDispose();
    }
}
