using Age.Numerics;

namespace Age.Styling;

public record Image
{
    public required string Uri;
    public ImageSize   Size;
    public ImageRepeat Repeat    = ImageRepeat.Repeat;
    public Transform2D Transform = Transform2D.Identity;
}
