using static Age.Shaders.CanvasShader;

namespace Age.Elements.Layouts;

#pragma warning disable CA1001 // TODO Remove;

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
