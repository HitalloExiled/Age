using Age.Numerics;
using Age.Rendering.Shaders.Canvas;

namespace Age.Styling;

public record struct Border
{
    // 4-bytes
    public BorderSide   Top;
    public BorderSide   Right;
    public BorderSide   Bottom;
    public BorderSide   Left;
    public BorderRadius Radius;

    public Border(uint thickness, uint radius, in Color color)
    {
        this.Top    = new(thickness, color);
        this.Right  = new(thickness, color);
        this.Bottom = new(thickness, color);
        this.Left   = new(thickness, color);
        this.Radius = new(radius);
    }

    public Border(in BorderSide horizontal, in BorderSide vertical, in BorderRadius borderRadius)
    {
        this.Top    = vertical;
        this.Right  = horizontal;
        this.Bottom = vertical;
        this.Left   = horizontal;
        this.Radius = borderRadius;
    }

    public Border(in BorderSide horizontal, in BorderSide vertical, uint radius = 0) : this(horizontal, vertical, new BorderRadius(radius))
    { }

    public static implicit operator CanvasShader.Border(in Border value) =>
        new()
        {
            Top = new()
            {
                Color     = value.Top.Color,
                Thickness = value.Top.Thickness,
            },
            Right = new()
            {
                Color     = value.Right.Color,
                Thickness = value.Right.Thickness,
            },
            Bottom = new()
            {
                Color     = value.Bottom.Color,
                Thickness = value.Bottom.Thickness,
            },
            Left = new()
            {
                Color     = value.Left.Color,
                Thickness = value.Left.Thickness,
            },

            Radius = new()
            {
                LeftTop     = value.Radius.LeftTop,
                TopRight    = value.Radius.TopRight,
                RightBottom = value.Radius.RightBottom,
                BottomLeft  = value.Radius.BottomLeft,
            }
        };
}
