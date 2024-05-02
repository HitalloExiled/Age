namespace Age.Rendering.Shaders;

public partial class CanvasShader
{
    public enum Flags : uint
    {
        Grayscale     = 1 << 0,
        MultiplyColor = 1 << 1,
    }
}
