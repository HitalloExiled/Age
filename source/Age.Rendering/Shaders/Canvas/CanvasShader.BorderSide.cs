using Age.Numerics;

namespace Age.Rendering.Shaders.Canvas;

public partial class CanvasShader
{
    public record struct BorderSide
    {
        public uint  Thickness;
        public Color Color;
    }
}
