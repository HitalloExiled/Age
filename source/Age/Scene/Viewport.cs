using Age.Numerics;
using Age.Rendering.Resources;

namespace Age.Scene;

public abstract class Viewport : Spatial2D
{
    public abstract event Action? Resized;

    public abstract Size<uint>   Size         { get; set; }
    public abstract RenderTarget RenderTarget { get; }
    public abstract Texture2D    Texture      { get; }
}
