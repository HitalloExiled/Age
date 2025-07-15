using static Age.Shaders.CanvasShader;

namespace Age.Elements;

public abstract partial class Element
{
    [Flags]
    private enum Dependency : byte
    {
        None    = 0,
        Width   = 1 << 0,
        Height  = 1 << 1,
        Margin  = 1 << 2,
        Padding = 1 << 3,
        Size    = Width | Height,
    }
}
