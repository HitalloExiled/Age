namespace Age.Drawing.Styling;

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

    public BorderRadius(uint top, uint bottom)
    {
        this.LeftTop     = top;
        this.TopRight    = top;
        this.RightBottom = bottom;
        this.BottomLeft  = bottom;
    }

    public BorderRadius(uint leftTop, uint topRight, uint rightBottom, uint bottomLeft)
    {
        this.LeftTop     = leftTop;
        this.TopRight    = topRight;
        this.RightBottom = rightBottom;
        this.BottomLeft  = bottomLeft;
    }
}
