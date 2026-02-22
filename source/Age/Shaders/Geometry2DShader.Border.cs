using Age.Numerics;

namespace Age.Shaders;

public partial class Geometry2DShader
{
    public struct Border
    {
        public BorderSide   Top;
        public BorderSide   Right;
        public BorderSide   Bottom;
        public BorderSide   Left;
        public BorderRadius Radius;

        public Border() { }

        public Border(ushort thickness, ushort radius, in Color color)
        {
            this.Top    = new(thickness, color);
            this.Right  = new(thickness, color);
            this.Bottom = new(thickness, color);
            this.Left   = new(thickness, color);
            this.Radius = new(radius);
        }

        public Border(uint thickness, uint radius, in Color color) : this((ushort)thickness, (ushort)radius, color)
        { }

        public Border(in BorderSide horizontal, in BorderSide vertical, in BorderRadius borderRadius)
        {
            this.Top    = vertical;
            this.Right  = horizontal;
            this.Bottom = vertical;
            this.Left   = horizontal;
            this.Radius = borderRadius;
        }

        public Border(in BorderSide horizontal, in BorderSide vertical, ushort radius = 0) : this(horizontal, vertical, new BorderRadius(radius))
        { }
    }
}
