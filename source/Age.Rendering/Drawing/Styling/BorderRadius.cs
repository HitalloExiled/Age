namespace Age.Rendering.Drawing.Styling;

public record struct BorderRadius
{
    public uint LeftTop;
    public uint TopRight;
    public uint RightBottom;
    public uint BottomLeft;

    public BorderRadius(uint radius)
    {
        this.LeftTop     = radius;
        this.TopRight    = radius;
        this.RightBottom = radius;
        this.BottomLeft  = radius;
    }

    public BorderRadius(uint leftTop, uint topRight, uint rightBottom, uint bottomLeft)
    {
        this.LeftTop     = leftTop;
        this.TopRight    = topRight;
        this.RightBottom = rightBottom;
        this.BottomLeft  = bottomLeft;
    }
}
