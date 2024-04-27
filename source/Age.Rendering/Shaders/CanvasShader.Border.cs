using Age.Numerics;

namespace Age.Rendering.Shaders;

public partial class CanvasShader
{
    public record struct Border
    {
        public uint           Size;
        public uint           Radius;
        public Color          Color;
        public BorderPosition Position;
    }
}
