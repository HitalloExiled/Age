using Age.Drawing.Styling;
using Age.Drawing;
using Age.Numerics;
using Age.Scene;

namespace Age.Editor;

public class EditorViewport3D : Element, IDisposable
{
    public override string NodeName { get; } = nameof(EditorViewport3D);

    private bool disposed;

    public Viewport Viewport { get; }

    public EditorViewport3D()
    {
        this.Style = new()
        {
            Border  = new Border(1, 0, Color.Red),
            MinSize = new Size<uint>(100)
        };

        this.AppendChild(this.Viewport = new Viewport(new(400)));
    }

    ~EditorViewport3D() =>
        this.Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            { }

            this.Viewport.Dispose();
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
