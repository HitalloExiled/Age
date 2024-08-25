using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements;

public abstract partial class Element
{
    [Flags]
    private enum Dependency
    {
        None    = 0,
        Width   = 1 << 0,
        Height  = 1 << 1,
        Margin  = 1 << 2,
        Padding = 1 << 3,
    }
}
