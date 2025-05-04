using Age.Resources;

namespace Age.Extensions;

public static partial class Extension
{
    public static int ClampSubtract(this int value, int other) =>
        value > other ? value - other : 0;

    public static float ClampSubtract(this float value, float other) =>
        value > other ? value - other : 0;

    public static int BytesPerPixel(this TextureFormat value) =>
        value switch
        {
            TextureFormat.B8G8R8A8Unorm => 4,
            TextureFormat.R8G8Unorm     => 2,
            TextureFormat.R8Unorm       => 1,
            _ => 0,
        };
}
