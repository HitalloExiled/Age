using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;
using Age.Rendering.Resources;

namespace Age.Rendering.Scene;

public class Viewport : Element, IDisposable
{
    private bool disposed;

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

    ~Viewport() =>
        this.Dispose(false);

    private void UpdateCommand()
    {
        if (this.SingleCommand is not RectCommand command)
        {
            this.SingleCommand = command = new();
        }

        command.Rect           = new Rect<float>(this.renderTarget.Size.Cast<float>(), default);
        command.SampledTexture = new(this.renderTarget.Texture, Container.Singleton.TextureStorage.DefaultSampler, UVRect.Normalized);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            { }

            this.RenderTarget.Dispose();

            this.disposed = true;
        }
    }

    protected override void OnDestroy() =>
        this.Dispose();

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
