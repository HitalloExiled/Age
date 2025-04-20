using Age.Numerics;

namespace Age.Elements;

public struct BoxModel
{
    public RectEdges  Border;
    public Rect<int>  Boundings;
    public Size<uint> Content;
    public RectEdges  Margin;
    public RectEdges  Padding;
}
