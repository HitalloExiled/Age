using System.Runtime.InteropServices;

namespace Age.Styling;

public enum UnitKind
{
    Pixel      = 1,
    Percentage = 2,
}

public readonly record struct Pixel(uint Value)
{
    public override readonly string ToString() => $"{this.Value}px";

    public static explicit operator Pixel(uint value) => new(value);
    public static implicit operator uint(Pixel pixel) => pixel.Value;
}

public readonly record struct Percentage(float Value)
{
    public readonly float Value = Value / 100;

    public override readonly string ToString() => $"{this.Value * 100}%";

    public static explicit operator Percentage(uint value) => new(value);
    public static implicit operator float(Percentage pixel) => pixel.Value;
}

[StructLayout(LayoutKind.Explicit)]
public readonly record struct Unit
{
    [FieldOffset(0)]
    public readonly UnitKind Kind;

    [FieldOffset(4)]
    private readonly Percentage percentage;

    [FieldOffset(4)]
    private readonly Pixel pixel;

    public Unit(Pixel pixel)
    {
        this.Kind  = UnitKind.Pixel;
        this.pixel = pixel;
    }

    public Unit(Percentage percentage)
    {
        this.Kind       = UnitKind.Percentage;
        this.percentage = percentage;
    }

    public readonly bool TryGetPercentage(out Percentage percentage)
    {
        if (this.Kind == UnitKind.Percentage)
        {
            percentage = this.percentage;

            return true;
        }

        percentage = default;

        return false;
    }

    public readonly bool TryGetPixel(out Pixel pixel)
    {
        if (this.Kind == UnitKind.Pixel)
        {
            pixel = this.pixel;

            return true;
        }

        pixel = default;

        return false;
    }

    public override readonly string ToString()
    {
        if (this.TryGetPercentage(out var percentage))
        {
            return percentage.ToString();
        }
        else if (this.TryGetPixel(out var pixel))
        {
            return pixel.ToString();
        }

        return "";
    }

    public static implicit operator Unit(Pixel pixel) => new(pixel);
    public static implicit operator Unit(Percentage percentage) => new(percentage);
}
