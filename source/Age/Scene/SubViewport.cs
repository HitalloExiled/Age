using Age.Commands;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Scene;

public class SubViewport : Viewport
{
    public override event Action? Resized;

    public override string NodeName => nameof(SubViewport);

    private RenderTarget renderTarget;

    public override RenderTarget RenderTarget => this.renderTarget;

    public override Size<uint> Size
    {
        get => this.RenderTarget.Size;
        set
        {
            if (this.RenderTarget.Size != value)
            {
                this.renderTarget.Dispose();
                this.renderTarget = this.CreateRenderTarget(value);

                this.UpdateCommand();

                this.Resized?.Invoke();
            }
        }
    }

    public Scene3D? Scene3D { get; set; }
    public Scene2D? Scene2D { get; set; }

    public Viewport? ParentViewport
    {
        get
        {
            for (var parent = this.Parent; parent != null; parent = parent.Parent)
            {
                if (parent is Viewport viewport)
                {
                    return viewport;
                }
            }

            return null;
        }
    }

    public Camera2D? Camera2D { get; set; }
    public Camera3D? Camera3D { get; set; }

    public SubViewport(in Size<uint> size)
    {
        this.renderTarget = this.CreateRenderTarget(size);
        this.UpdateCommand();
    }

    private RenderTarget CreateRenderTarget(Size<uint> size)
    {
        var createInfo = new RenderTarget.CreateInfo
        {
            Size = size,
            ColorAttachments =
                    [
                        new()
                {
                    Format      = TextureFormat.B8G8R8A8Unorm,
                    SampleCount = SampleCount.N1,
                    Usage       = TextureUsage.ColorAttachment | TextureUsage.Sampled,
                }
                    ],
            DepthStencilAttachment = new()
            {
                Format = (TextureFormat)VulkanRenderer.Singleton.DepthBufferFormat,
                Aspect = TextureAspect.Stencil,
            }
        };

        return new(createInfo);
    }

    private void UpdateCommand()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = CommandPool.RectCommand.Get();
        }

        command.Size       = this.RenderTarget.Size;
        command.TextureMap = new(this.RenderTarget.ColorAttachments[0].Texture, UVRect.Normalized);
    }

    private protected override void OnDisposedInternal()
    {
        base.OnDisposedInternal();

        this.RenderTarget.Dispose();
    }
}
