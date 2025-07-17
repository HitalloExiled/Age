using Age.Numerics;

namespace Age.Extensions;

public static partial class Extension
{
    extension(in Size<uint> value)
    {
        public Size<uint> ClampSubtract(in Size<uint> other) =>
            new(
                value.Width.ClampSubtract(other.Width),
                value.Height.ClampSubtract(other.Height)
            );
    }

    extension(in Size<float> value)
    {
        public Size<float> ClampSubtract(in Size<float> other) =>
            new(
                value.Width.ClampSubtract(other.Width),
                value.Height.ClampSubtract(other.Height)
            );
    }
}
