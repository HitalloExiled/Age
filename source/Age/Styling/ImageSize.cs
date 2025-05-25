using Age.Numerics;

namespace Age.Styling;

public readonly struct ImageSize
{
    public readonly ImageSizeKind Kind;
    public readonly SizeUnit      Value;

    private ImageSize(ImageSizeKind kind) =>
        this.Kind = kind;

    private ImageSize(SizeUnit size) : this(ImageSizeKind.Size) =>
        this.Value = size;

    public static ImageSize Fit() =>
        new(ImageSizeKind.Fit);

    public static ImageSize KeepAspect() =>
        new(ImageSizeKind.KeepAspect);

    public static ImageSize Size(SizeUnit size) =>
        new(size);

    public static ImageSize Size(Unit? size) =>
        new(new SizeUnit(size));

    public static ImageSize Size(Unit? width, Unit? height) =>
        new(new SizeUnit(width, height));
}
