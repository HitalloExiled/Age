namespace Age.Styling;

public record struct BorderRadius
{
    public ushort LeftTop;
    public ushort TopRight;
    public ushort RightBottom;
    public ushort BottomLeft;

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

    public override readonly string ToString() =>
        $"[{this.LeftTop}, {this.TopRight}, {this.RightBottom}, ${this.BottomLeft}]";
}
