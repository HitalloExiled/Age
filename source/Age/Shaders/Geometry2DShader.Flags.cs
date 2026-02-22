namespace Age.Shaders;

public partial class Geometry2DShader
{
    [Flags]
    public enum Flags : uint
    {
        None              = 0,
        GrayscaleTexture  = 1 << 0,
        MultiplyColor     = 1 << 1,
        ColorAsBackground = 1 << 2,
        SamplerAsEncode   = 1 << 3,
    }
}
