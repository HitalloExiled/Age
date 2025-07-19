using static Age.Shaders.CanvasShader;

namespace Age.Elements;

public abstract partial class Element
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
