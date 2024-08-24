using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements;

public abstract partial class Element
{
    [Flags]
    private enum Dependency
    {
        None                    = 0,
        ContentWidth            = 1 << 0,
        ContentHeight           = 1 << 1,
        ParentWidth             = 1 << 2,
        ParentHeight            = 1 << 3,
        ParentMarginVertical    = 1 << 4,
        ParentMarginHorizontal  = 1 << 5,
        ParentPaddingVertical   = 1 << 6,
        ParentPaddingHorizontal = 1 << 7,
    }
}
