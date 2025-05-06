using Age.Resources;

namespace Age.Extensions;

public static partial class Extension
{
    public static int BytesPerPixel(this TextureFormat value) =>
        value switch
        {
            TextureFormat.B8G8R8A8Unorm => 4,
            TextureFormat.R8G8Unorm     => 2,
            TextureFormat.R8Unorm       => 1,
            _ => 0,
        };
}
