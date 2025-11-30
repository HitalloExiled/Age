using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Scenes;

public sealed class SubViewport : Viewport
{
    public override event Action? Resized;

    private readonly RenderGraph renderGraph;

    private RenderTarget renderTarget;

    public override Size<uint> Size
    {
        get => this.RenderTarget.Size;
        set
        {
            if (this.RenderTarget.Size != value)
            {
                this.renderTarget.Dispose();
                this.renderTarget = CreateRenderTarget(value);

                this.Resized?.Invoke();
            }
        }
    }

    public override string       NodeName     => nameof(SubViewport);
    public override RenderTarget RenderTarget => this.renderTarget;
    public override RenderGraph  RenderGraph => this.renderGraph;
    public override Texture2D    Texture      => this.RenderTarget.ColorAttachments[0].Texture;

    public SubViewport(in Size<uint> size)
    {
        this.renderTarget = CreateRenderTarget(size);
        this.renderGraph  = RenderGraph.CreateDefault(this);
    }

    private static RenderTarget CreateRenderTarget(Size<uint> size)
    {
        var createInfo = new RenderTarget.CreateInfo
        {
            Size             = size,
            ColorAttachments =
            [
                new()
                {
                    FinalLayout = ImageLayout.ShaderReadOnlyOptimal,
                    Format      = TextureFormat.B8G8R8A8Unorm,
                    SampleCount = SampleCount.N1,
                    Usage       = TextureUsage.ColorAttachment | TextureUsage.Sampled,
                }
            ],
            DepthStencilAttachment = new()
            {
                FinalLayout = ImageLayout.DepthStencilAttachmentOptimal,
                Format      = VulkanRenderer.Singleton.DepthBufferFormat,
                Aspect      = TextureAspect.Depth,
            }
        };

        return new(createInfo);
    }

    private protected override void OnDisposedInternal()
    {
        base.OnDisposedInternal();

        this.RenderTarget.Dispose();
        this.RenderGraph.Dispose();
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (this.Parent is not Scenes.Scene)
        {
            throw new InvalidOperationException("SubViewport must be attached directly to an Scene.");
        }
    }
}
