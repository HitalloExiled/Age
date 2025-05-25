using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Resources;

namespace Age.Scene;

public sealed class Viewport : Element
{
    public override string NodeName => nameof(Viewport);

    public RenderTarget RenderTarget { get; }

    public Size<uint> ViewSize
    {
        get => this.RenderTarget.Size;
        set
        {
            if (this.RenderTarget.Size != value)
            {
                this.Style.MinSize = new(Unit.Px(value.Width), Unit.Px(value.Height));
                this.RenderTarget.Update(value);
                this.UpdateCommand();
            }
        }
    }

    public Viewport(in Size<uint> size)
    {
        this.Style.MinSize = new(Unit.Px(size.Width), Unit.Px(size.Height));
        this.RenderTarget  = new(size);
        this.UpdateCommand();
    }

    private void UpdateCommand()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = new();
        }

        command.Size          = this.RenderTarget.Size.Cast<float>();
        command.MappedTexture = new(this.RenderTarget.Texture, UVRect.Normalized);
    }

    protected override void OnDisposed() =>
        this.RenderTarget.Dispose();
}
