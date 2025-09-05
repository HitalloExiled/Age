namespace Age.Shaders;

public partial class CanvasShader
{
    [Flags]
    public enum Flags : uint
    {
        None              = 0,
        GrayscaleTexture  = 1 << 0,
        MultiplyColor     = 1 << 1,
        ColorAsBackground = 1 << 2,
    }
}
