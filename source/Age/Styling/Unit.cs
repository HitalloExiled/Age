using System.Runtime.InteropServices;

namespace Age.Styling;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Unit
{
    [FieldOffset(0)]
    internal Kind kind;

    [FieldOffset(4)]
    internal Data data;

    public static Unit Em(float em) =>
        new()
        {
            kind = Kind.Em,
            data = new() { Em = em },
        };

    public static Unit Pc(float percentage) =>
        new()
        {
            kind = Kind.Percentage,
            data = new() { Percentage = percentage / 100 },
        };

    public static Unit Px(int pixel) =>
        new()
        {
            kind = Kind.Pixel,
            data = new() { Pixel = pixel },
        };

    public static Unit Px(uint pixel) =>
        Px((int)pixel);

    internal static float Resolve(Unit? unit, uint size, uint fontSize)
    {
        if (!unit.HasValue)
        {
            return 0;
        }

        return unit.Value.kind switch
        {
            Kind.Pixel      => unit.Value.data.Pixel,
            Kind.Percentage => unit.Value.data.Percentage * size,
            Kind.Em         => unit.Value.data.Em * fontSize,
            _ => throw new NotSupportedException(),
        };
    }

    public readonly bool TryGetEm(out float em)
    {
        if (this.kind == Kind.Em)
        {
            em = this.data.Em;

            return true;
        }

        em = default;

        return false;
    }

    public readonly bool TryGetPercentage(out float percentage)
    {
        if (this.kind == Kind.Percentage)
        {
            percentage = this.data.Percentage;

            return true;
        }

        percentage = default;

        return false;
    }

    public readonly bool TryGetPixel(out int pixel)
    {
        if (this.kind == Kind.Pixel)
        {
            pixel = this.data.Pixel;

            return true;
        }

        pixel = default;

        return false;
    }

    public override readonly string ToString() =>
        this.kind switch
        {
            Kind.Em         => $"{this.data.Em}em",
            Kind.Pixel      => $"{this.data.Pixel}px",
            Kind.Percentage => $"{this.data.Percentage * 100}%",
            _ => "",
        };
}
