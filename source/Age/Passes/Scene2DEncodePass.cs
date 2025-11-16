using System.Diagnostics.CodeAnalysis;
using Age.Commands;
using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using Age.Shaders;

namespace Age.Passes;

public sealed class Scene2DEncodePass : Scene2DPass
{
    private readonly Window window;

    private RenderTarget renderTarget;

    protected override Shader Shader { get; }

    protected override Color         ClearColor    => Color.Black;
    protected override CommandBuffer CommandBuffer => VulkanRenderer.Singleton.CurrentCommandBuffer;
    protected override RenderTarget  RenderTarget  => this.renderTarget;
    public override Texture2D        Output        => this.renderTarget.ColorAttachments[0].Texture;

    public Scene2DEncodePass(Window window) : base(window)
    {
        this.window = window;

        this.CreateRenderTarget();

        window.Resized += this.CreateRenderTarget;

        this.Shader = new CanvasIndexShader(renderTarget, true);
    }

    [MemberNotNull(nameof(renderTarget))]
    private void CreateRenderTarget()
    {
        var createInfo = new RenderTarget.CreateInfo
        {
            Size             = this.window.Size,
            ColorAttachments =
            [
                new()
                {
                    FinalLayout = ThirdParty.Vulkan.Enums.VkImageLayout.ShaderReadOnlyOptimal,
                    SampleCount = SampleCount.N1,
                    Format      = TextureFormat.R16G16B16A16Unorm,
                    Usage       = TextureUsage.TransferDst | TextureUsage.TransferSrc | TextureUsage.Sampled | TextureUsage.ColorAttachment
                }
            ]
        };

        this.renderTarget = new(createInfo);
    }

    protected override void Record(RenderContext context) =>
        this.Record(context.Buffer2D.Commands);

    protected override void Record(RectCommand command)
    {
        var constant = new CanvasShader.PushConstant
        {
            Border    = command.Border,
            Color     = 0xFFFF_0000_0000_0000 | command.Metadata,
            Flags     = command.Flags,
            Size      = command.Size,
            Transform = command.Matrix,
            UV        = UVRect.Normalized,
            Viewport  = this.Viewport.Size,
        };

        this.CommandBuffer.PushConstant(this.Shader, constant);
        this.CommandBuffer.DrawIndexed(this.IndexBuffer);
    }

    protected override void OnDisposed(bool disposing)
    {
        this.renderTarget.Dispose();
        this.Shader.Dispose();
    }
}
