using Age.Numerics;

namespace Age.Shaders;

public partial class CanvasShader
{
    public record struct BorderSide
    {
        public uint  Thickness;
        public Color Color;
    }
}
