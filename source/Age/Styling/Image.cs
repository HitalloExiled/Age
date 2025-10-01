using Age.Rendering.Resources;

namespace Age.Styling;

public record Image
{
    internal Texture2D? Texture { get; }

    public string? Uri { get; }

    public ImageSize   Size     { get; init; }
    public ImageRepeat Repeat   { get; init; } = ImageRepeat.Repeat;
    public PointUnit   Position { get; init; }

    public Image(string uri) =>
        this.Uri = uri;

    public Image(Texture2D texture) =>
        this.Texture = texture;
}
