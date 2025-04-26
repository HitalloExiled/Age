using static Age.Shaders.CanvasShader;

namespace Age.Elements.Layouts;

internal partial class BoxLayout
{
    [Flags]
    private enum LayoutCommand
    {
        None     = 0,
        Box      = 1 << 0,
        Image    = 1 << 1,
        ScrollX  = 1 << 2,
        ScrollY  = 1 << 3,
    }
}
