using static Age.Shaders.CanvasShader;

namespace Age.Elements.Layouts;

internal partial class BoxLayout
{
    [Flags]
    private enum AlignmentAxis
    {
        Baseline   = 1 << 0,
        Horizontal = 1 << 1,
        Vertical   = 1 << 2,
    }
}
