using Age.Numerics;
using Age.Resources;

namespace Age.Scene;

public class ViewportV2(in Size<uint> size) : Spatial2D
{
    internal RenderTarget RenderTarget { get; } = new(size);

    public override string NodeName => nameof(ViewportV2);

    public CanvasV2 Canvas { get; } = new();

    public ViewportV2? ParentViewport => this.Parent as ViewportV2;

    public Texture2D Texture => this.RenderTarget.Texture;

    public Size<uint> Size
    {
        get => this.RenderTarget.Size;
        set
        {
            if (this.RenderTarget.Size != value)
            {
                this.RenderTarget.Update(value);
            }
        }
    }

    public ViewportV2? RootViewport
    {
        get
        {
            for (var current = this; ;current = current.ParentViewport)
            {
                if (current.ParentViewport == null)
                {
                    return current;
                }
            }
        }
    }
}
