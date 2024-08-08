using Age.Commands;
using Age.Elements;
using Age.Numerics;
using Age.Resources;
using Age.Storage;

namespace Age.Scene;

#pragma warning disable CA1001

public class Viewport : Element
{
    private readonly RenderTarget renderTarget;

    public override string NodeName => nameof(Viewport);

    public RenderTarget RenderTarget => this.renderTarget;

    public Size<uint> ViewSize
    {
        get => this.renderTarget.Size;
        set
        {
            if (this.renderTarget.Size != value)
            {
                this.Style.MinSize = value;
                this.renderTarget.Update(value);
                this.UpdateCommand();
            }
        }
    }

    public Viewport(in Size<uint> size)
    {
        this.Style.MinSize = size;
        this.renderTarget = new(size);
        this.UpdateCommand();
    }

    private void UpdateCommand()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = new();
        }

        command.Rect           = new Rect<float>(this.renderTarget.Size.Cast<float>(), default);
        command.SampledTexture = new(this.renderTarget.Texture, TextureStorage.Singleton.DefaultSampler, UVRect.Normalized);
    }

    protected override void Destroyed() =>
        this.RenderTarget.Dispose();
}
