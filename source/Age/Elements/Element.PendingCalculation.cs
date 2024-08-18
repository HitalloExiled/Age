using static Age.Rendering.Shaders.Canvas.CanvasShader;

namespace Age.Elements;

public abstract partial class Element
{
    [Flags]
    private enum Dependency
    {
        None         = 0,
        ChildWidth   = 1 << 0,
        ChildHeight  = 1 << 1,
        ParentWidth  = 1 << 2,
        ParentHeight = 1 << 3,
        AllChild     = ChildWidth | ChildHeight,
        AllParent    = ParentWidth | ParentHeight,
    }
}
