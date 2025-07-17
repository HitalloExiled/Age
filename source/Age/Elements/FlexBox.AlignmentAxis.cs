using static Age.Shaders.CanvasShader;

namespace Age.Elements;

public sealed partial class FlexBox
{
    [Flags]
    private enum AlignmentAxis
    {
        None       = 0,
        Baseline   = 1 << 0,
        Horizontal = 1 << 1,
        Vertical   = 1 << 2,
    }
}
