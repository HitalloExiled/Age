using Age.Numerics;

namespace Age.Extensions;

public static class SizeExtensions
{
    public static Size<uint> ClampSubtract(this in Size<uint> value, in Size<uint> other) =>
        new(
            value.Width.ClampSubtract(other.Width),
            value.Height.ClampSubtract(other.Height)
        );

    public static Size<float> ClampSubtract(this in Size<float> value, in Size<float> other) =>
        new(
            value.Width.ClampSubtract(other.Width),
            value.Height.ClampSubtract(other.Height)
        );
}
