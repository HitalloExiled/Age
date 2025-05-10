namespace Age.Styling;

public record Image
{
    public required string Uri;
    public ImageSize   Size;
    public ImageRepeat Repeat    = ImageRepeat.Repeat;
    public PointUnit   Position;
}
