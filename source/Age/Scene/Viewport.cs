using Age.Numerics;
using Age.Resources;

namespace Age.Scene;

public abstract class Viewport : Spatial2D
{
    public abstract event Action? Resized;

    public abstract Size<uint>   Size         { get; set; }
    public abstract RenderTarget RenderTarget { get; }
}
