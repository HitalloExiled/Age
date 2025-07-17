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

        var leftTop     = uint.Min(border.Radius.LeftTop,     minRadius);
        var topRight    = uint.Min(border.Radius.TopRight,    minRadius);
        var rightBottom = uint.Min(border.Radius.RightBottom, minRadius);
        var bottomLeft  = uint.Min(border.Radius.BottomLeft,  minRadius);

        bool tryCreateEllipse(uint radius, Point<uint> origin, Size<uint> thickness, float startAngle)
        {
            if (radius > thickness.Width && radius > thickness.Height)
            {
                origin.X = (origin.X * (bounds.Width  - (radius * 2))) + radius;
                origin.Y = (origin.Y * (bounds.Height - (radius * 2))) + radius;

                var radiusX = radius - thickness.Width;
                var radiusY = radius - thickness.Height;

                var rect = new SKRect(origin.X - radiusX, origin.Y - radiusY, origin.X + radiusX, origin.Y + radiusY);

                path.ArcTo(rect, startAngle, 90, false);

                return true;
            }

            return false;
        }

        path.MoveTo(uint.Max(border.Left.Thickness, leftTop), border.Top.Thickness);

        if (topRight == 0 || !tryCreateEllipse(topRight, new(1, 0), new(border.Right.Thickness, border.Top.Thickness), 270))
        {
            path.LineTo(bounds.Width - border.Right.Thickness, border.Top.Thickness);
        }

        if (rightBottom == 0 || !tryCreateEllipse(rightBottom, new(1, 1), new(border.Right.Thickness, border.Bottom.Thickness), 0))
        {
            path.LineTo(bounds.Width - border.Right.Thickness, bounds.Height - border.Bottom.Thickness);
        }

        if (bottomLeft == 0 || !tryCreateEllipse(bottomLeft, new(0, 1), new(border.Left.Thickness, border.Bottom.Thickness), 90))
        {
            path.LineTo(border.Left.Thickness, bounds.Height - border.Bottom.Thickness);
        }

        if (leftTop == 0 || !tryCreateEllipse(leftTop, new(0, 0), new(border.Left.Thickness, border.Top.Thickness), 180))
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
            Radius = new()
            {
                LeftTop     = 10,
                TopRight    = 20,
                RightBottom = 30,
                BottomLeft  = 40,
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

        canvas.Scale(1, -1);
        canvas.Translate(0, -size.Height);

        var flipAndTranslateMatrix = SKMatrix.CreateScale(1, -1);

        flipAndTranslateMatrix.TransY = size.Height;

        path.Transform(flipAndTranslateMatrix);

        canvas.DrawPath(path, paint);

        SaveImage(bitmap, "path.png");
    }
}
