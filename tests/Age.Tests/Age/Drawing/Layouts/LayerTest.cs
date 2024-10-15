using Age.Numerics;
using Age.Styling;
using SkiaSharp;

using Common = Age.Internal.Common;

namespace Age.Tests.Age.Drawing.Layouts;

public class LayerTest
{
    private static SKPath CreatePath(Size<uint> bounds, Border border)
    {
        var path = new SKPath();

        // var border = this.Owner.Layout.State.Style.Border ?? default;
        // var bounds = this.Owner.Layout.Boundings;

        var minRadius = uint.Min(bounds.Width, bounds.Height) / 2;

        bool tryCreateEllipse(uint radius, Point<uint> position, Size<uint> thickness, float startAngle)
        {
            var targetRadius = uint.Min(radius, minRadius);

            if (targetRadius > thickness.Width && targetRadius > thickness.Height)
            {
                var diameter = targetRadius * 2;
                var radiusX  = diameter - thickness.Width;
                var radiusY  = diameter - thickness.Height;
                var origin   = (new Point<uint>(bounds.Width, bounds.Height) - new Point<uint>(targetRadius)) * position;

                origin.X = position.X == 0 ? origin.X + thickness.Width : origin.X - thickness.Width;
                origin.Y = position.Y == 0 ? origin.Y + thickness.Height : origin.Y - thickness.Height;

                var rect = SKRect.Create(origin.X, origin.Y, radiusX, radiusY);

                path.ArcTo(rect, startAngle, 90, false);

                return true;
            }

            return false;
        }

        path.MoveTo(uint.Max(border.Left.Thickness, border.Radius.LeftTop), border.Top.Thickness);

        if (border.Radius.TopRight == 0 || !tryCreateEllipse(border.Radius.TopRight, new(1, 0), new(border.Right.Thickness, border.Top.Thickness), 270))
        {
            path.LineTo(bounds.Width - border.Right.Thickness, border.Top.Thickness);
        }

        if (border.Radius.RightBottom == 0 || !tryCreateEllipse(border.Radius.RightBottom, new(1, 1), new(border.Right.Thickness, border.Bottom.Thickness), 0))
        {
            path.LineTo(bounds.Width - border.Right.Thickness, bounds.Height - border.Bottom.Thickness);
        }

        if (border.Radius.BottomLeft == 0 || !tryCreateEllipse(border.Radius.BottomLeft, new(0, 1), new(border.Left.Thickness, border.Bottom.Thickness), 90))
        {
            path.LineTo(border.Left.Thickness, bounds.Height - border.Bottom.Thickness);
        }

        if (border.Radius.LeftTop == 0 || !tryCreateEllipse(border.Radius.LeftTop, new(0, 0), new(border.Left.Thickness, border.Top.Thickness), 180))
        {
            path.LineTo(border.Left.Thickness, border.Top.Thickness);
        }

        return path;
    }

    private static void SaveImage(SKBitmap bitmap, string filename)
    {
        var debugDirectory = Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), "../../../../../.debug"));

        Directory.CreateDirectory(debugDirectory);

        Common.SaveImage(bitmap, Path.Join(debugDirectory, filename));
    }

    [Fact]
    public void TestName()
    {
        var size = new Size<uint>(200, 100);
        var border = new Border
        {
            Top    = new(10, default),
            Right  = new(10, default),
            Bottom = new(10, default),
            Left   = new(10, default),
            Radius =
            {
                LeftTop     = 20,
                TopRight    = 20,
                RightBottom = 0,
                BottomLeft  = 0,
            }
        };

        using var path = CreatePath(size, border);

        using var bitmap = new SKBitmap((int)size.Width, (int)size.Height);
        using var canvas = new SKCanvas(bitmap);
        using var paint  = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill,
        };

        canvas.DrawPath(path, paint);

        SaveImage(bitmap, "path.png");
    }
}
