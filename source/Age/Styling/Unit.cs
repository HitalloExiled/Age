using System.Collections;
using System.Runtime.InteropServices;

namespace Age.Styling;

public enum UnitKind
{
    Pixel      = 1,
    Percentage = 2,
    Em         = 3,
}

public readonly record struct Em(float Value)
{
    public override readonly string ToString() => $"{this.Value}em";

    public static explicit operator Em(float value) => new(value);
    public static implicit operator float(Em em) => em.Value;
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

    public static explicit operator Percentage(float value) => new(value);
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

    [FieldOffset(4)]
    private readonly Em em;

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

    public Unit(Em em)
    {
        this.Kind = UnitKind.Em;
        this.em   = em;
    }

    public static Em         Em(float value) => new(value);
    public static Percentage Pc(float value) => new(value);
    public static Pixel      Px(uint value) => new(value);

    public readonly bool TryGetEm(out Em em)
    {
        if (this.Kind == UnitKind.Em)
        {
            em = this.em;

            return true;
        }

        em = default;

        return false;
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

    public override readonly string ToString() =>
        this.Kind switch
        {
            UnitKind.Em         => this.em.ToString(),
            UnitKind.Pixel      => this.pixel.ToString(),
            UnitKind.Percentage => this.percentage.ToString(),
            _ => "",
        };

    public static implicit operator Unit(Em em) => new(em);
    public static implicit operator Unit(Pixel pixel) => new(pixel);
    public static implicit operator Unit(Percentage percentage) => new(percentage);
}
