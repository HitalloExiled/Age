using Age.Numerics;

namespace Age.Shaders;

public partial class CanvasShader
{
    public record struct BorderSide
    {
        public uint  Thickness;
        public Color Color;

        public BorderSide(ushort thickness, Color color)
        {
            this.Thickness = thickness;
            this.Color     = color;
        }
    }
}
