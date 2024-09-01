using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements.Layouts;

internal partial class BoxLayout
{
    [Flags]
    public enum Dependency
    {
        None    = 0,
        Width   = 1 << 0,
        Height  = 1 << 1,
        Margin  = 1 << 2,
        Padding = 1 << 3,
    }
}
