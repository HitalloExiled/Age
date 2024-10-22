using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Resources;
using Age.Styling;

namespace Age.Scene;

public class Viewport : Element
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
                this.Style.MinSize = new((Pixel)value.Width, (Pixel)value.Height);
                this.RenderTarget.Update(value);
                this.UpdateCommand();
            }
        }
    }

    public Viewport(in Size<uint> size)
    {
        this.Style.MinSize = new((Pixel)size.Width, (Pixel)size.Height);
        this.RenderTarget  = new(size);
        this.UpdateCommand();
    }

    private void UpdateCommand()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = new()
            {
                Id       = this.GetHashCode(),
                MappedTexture  = default!,
            };
        }

        command.Rect    = new Rect<float>(this.RenderTarget.Size.Cast<float>(), default);
        command.MappedTexture = new(this.RenderTarget.Texture, UVRect.Normalized);
    }

    protected override void Disposed() =>
        this.RenderTarget.Dispose();
}
