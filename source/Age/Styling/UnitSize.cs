using System.Numerics;
using Age.Numerics;

namespace Age.Styling;

public record struct SizeUnit
{
    public Unit? Width;
    public Unit? Height;

    public SizeUnit(Unit? value) : this(value, value)
    { }

    public SizeUnit(Unit? width, Unit? height)
    {
        this.Width  = width;
        this.Height = height;
    }

    public SizeUnit(uint? value) : this(value, value)
    { }

    public SizeUnit(uint? width, uint? height)
    {
        this.Width  = width.HasValue  ? Unit.Pixel(width.Value) : null;
        this.Height = height.HasValue ? Unit.Pixel(height.Value) : null;
    }

    public static SizeUnit Pixel(float value) =>
        new(Unit.Pixel(value));

    public static SizeUnit Pixel<T>(T value) where T : INumber<T> =>
        new(Unit.Pixel(float.CreateSaturating(value)));

    public static SizeUnit Pixel(Size<float> value) =>
        new(Unit.Pixel(value.Width), Unit.Pixel(value.Height));

    public static SizeUnit Pixel<T>(Size<T> value) where T : INumber<T> =>
        new(Unit.Pixel(float.CreateTruncating(value.Width)), Unit.Pixel(float.CreateTruncating(value.Height)));

    public static SizeUnit Pixel(float width, float height) =>
        new(Unit.Pixel(width), Unit.Pixel(height));

    public static SizeUnit Percentage(float value) =>
        new(Unit.Percentage(value));

    public static SizeUnit Percentage<T>(T value) where T : INumber<T> =>
        new(Unit.Percentage(float.CreateSaturating(value)));

    public static SizeUnit Percentage(Size<float> value) =>
        new(Unit.Percentage(value.Width), Unit.Percentage(value.Height));

    public static SizeUnit Percentage<T>(Size<T> value) where T : INumber<T> =>
        new(Unit.Percentage(float.CreateTruncating(value.Width)), Unit.Percentage(float.CreateTruncating(value.Height)));

    public static SizeUnit Percentage(float width, float height) =>
        new(Unit.Pixel(width), Unit.Pixel(height));

    public override readonly string ToString() =>
        $"Width: {this.Width}, Height: {this.Height}";
}
