namespace Age.Rendering.Shaders;

public partial class CanvasShader
{
    public enum Flags : uint
    {
        GrayscaleTexture  = 1 << 0,
        MultiplyColor     = 1 << 1,
        ColorAsBackground = 1 << 2,
    }
}
