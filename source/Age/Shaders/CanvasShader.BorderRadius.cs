namespace Age.Shaders;

public partial class CanvasShader
{
    public record struct BorderRadius
    {
        public uint LeftTop;
        public uint TopRight;
        public uint RightBottom;
        public uint BottomLeft;
    }
}
