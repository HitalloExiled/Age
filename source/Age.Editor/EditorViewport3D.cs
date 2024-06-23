using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Drawing.Styling;
using Age.Rendering.Resources;
using Age.Rendering.Scene;

namespace Age.Editor;

public class EditorViewport3D : Element, IDisposable
{
    public override string NodeName { get; } = nameof(EditorViewport3D);

    private readonly Viewport viewport;

    private bool disposed;

    public RenderTarget RenderTarget => this.viewport.RenderTarget;

    public EditorViewport3D()
    {
        this.Style = new()
        {
            Border  = new Border(1, 0, Color.Red),
            MinSize = new Size<uint>(400)
        };

        this.AppendChild(this.viewport = new Viewport(new(400)));
    }

    ~EditorViewport3D() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            { }

            this.viewport.Dispose();
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
