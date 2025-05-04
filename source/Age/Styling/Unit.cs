using System.Runtime.InteropServices;

namespace Age.Styling;

[StructLayout(LayoutKind.Explicit)]
public partial record struct Unit
{
    [FieldOffset(0)]
    internal UnitKind Kind;

    [FieldOffset(4)]
    internal UnitData Data;

    public static Unit Em(float em) =>
        new()
        {
            Kind = UnitKind.Em,
            Data = new() { Em = em },
        };

    public static Unit Pc(float percentage) =>
        new()
        {
            Kind = UnitKind.Percentage,
            Data = new() { Percentage = percentage / 100 },
        };

    public static Unit Px(int pixel) =>
        new()
        {
            Kind = UnitKind.Pixel,
            Data = new() { Pixel = pixel },
        };

    public static Unit Px(uint pixel) =>
        Px((int)pixel);

    internal static float Resolve(Unit? unit, uint size, uint fontSize)
    {
        if (!unit.HasValue)
        {
            return 0;
        }

        return unit.Value.Kind switch
        {
            UnitKind.Pixel      => unit.Value.Data.Pixel,
            UnitKind.Percentage => unit.Value.Data.Percentage * size,
            UnitKind.Em         => unit.Value.Data.Em * fontSize,
            _               => unit.Value.Data.Pixel,
        };
    }

    public readonly bool TryGetEm(out float em)
    {
        if (this.Kind == UnitKind.Em)
        {
            em = this.Data.Em;

            return true;
        }

        em = default;

        return false;
    }

    public readonly bool TryGetPercentage(out float percentage)
    {
        if (this.Kind == UnitKind.Percentage)
        {
            percentage = this.Data.Percentage;

            return true;
        }

        percentage = default;

        return false;
    }

    public readonly bool TryGetPixel(out int pixel)
    {
        if (this.Kind == UnitKind.Pixel)
        {
            pixel = this.Data.Pixel;

            return true;
        }

        pixel = default;

        return false;
    }

    public override readonly string ToString() =>
        this.Kind switch
        {
            UnitKind.Em         => $"{this.Data.Em}em",
            UnitKind.Pixel      => $"{this.Data.Pixel}px",
            UnitKind.Percentage => $"{this.Data.Percentage * 100}%",
            _ => "",
        };
}
