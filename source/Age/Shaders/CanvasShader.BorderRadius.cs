namespace Age.Shaders;

public partial class CanvasShader
{
    public record struct BorderRadius
    {
        public uint LeftTop;
        public uint TopRight;
        public uint RightBottom;
        public uint BottomLeft;

        public BorderRadius(ushort radius)
        {
            this.LeftTop     = radius;
            this.TopRight    = radius;
            this.RightBottom = radius;
            this.BottomLeft  = radius;
        }

        public BorderRadius(ushort top, ushort bottom)
        {
            this.LeftTop     = top;
            this.TopRight    = top;
            this.RightBottom = bottom;
            this.BottomLeft  = bottom;
        }

        public BorderRadius(ushort leftTop, ushort topRight, ushort rightBottom, ushort bottomLeft)
        {
            this.LeftTop     = leftTop;
            this.TopRight    = topRight;
            this.RightBottom = rightBottom;
            this.BottomLeft  = bottomLeft;
        }
    }
}
