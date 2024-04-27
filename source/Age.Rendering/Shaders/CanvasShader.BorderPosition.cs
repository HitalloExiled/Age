namespace Age.Rendering.Shaders;

public partial class CanvasShader
{
    public enum BorderPosition : uint
    {
        Top    = 1 << 0,
        Right  = 1 << 1,
        Bottom = 1 << 2,
        Left   = 1 << 3,
        All    = Top | Right | Bottom | Left
    }
}
