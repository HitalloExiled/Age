using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Rendering.Vulkan;
using Age.Resources;

namespace Age.Scene;

public sealed class Viewport : Element
{
    public override string NodeName => nameof(Viewport);

    public RenderTarget RenderTarget { get; }

    public Size<uint> ViewSize
    {
        get => this.RenderTarget.Size;
        set => this.RenderTarget.Size = value;
    }

    public Viewport(in Size<uint> size)
    {
        this.Style.MinSize = new(Unit.Px(size.Width), Unit.Px(size.Height));

        var createInfo = new RenderTarget.CreateInfo
        {
            Size             = size,
            ColorAttachments =
            [
                new()
                {
                    Format = TextureFormat.B8G8R8A8Unorm,
                    Usage  = TextureUsage.ColorAttachment | TextureUsage.Sampled,
                }
            ],
            DepthStencilAttachment = new()
            {
                Format = (TextureFormat)VulkanRenderer.Singleton.DepthBufferFormat,
            }
        };

        this.RenderTarget  = new(createInfo);
        this.UpdateCommand();
    }

    private void UpdateCommand()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = new();
        }

        command.Size       = this.RenderTarget.Size.Cast<float>();
        command.TextureMap = new(this.RenderTarget.ColorAttachments[0].Texture, UVRect.Normalized);
    }

    protected override void OnDisposed() =>
        this.RenderTarget.Dispose();
}
