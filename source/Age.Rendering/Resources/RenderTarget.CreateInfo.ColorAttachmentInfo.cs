namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public struct ColorAttachmentInfo : IEquatable<ColorAttachmentInfo>
        {
            internal Image? Image;

            public required ImageLayout   FinalLayout;
            public required TextureFormat Format;
            public required SampleCount   SampleCount;

            public TextureUsage Usage;
            public bool         EnableResolve;

            internal static ColorAttachmentInfo From(Image image, ImageLayout finalLayout) =>
                new()
                {
                    Image       = image,
                    Format      = (TextureFormat)image.Format,
                    SampleCount = (SampleCount)image.Samples,
                    Usage       = (TextureUsage)image.Usage,
                    FinalLayout = finalLayout,
                };

            public override readonly int GetHashCode() =>
                HashCode.Combine(
                    this.FinalLayout,
                    this.Format,
                    this.SampleCount,
                    this.Usage,
                    this.EnableResolve
                );

            public readonly bool Equals(ColorAttachmentInfo other) =>
                this.FinalLayout      == other.FinalLayout
                && this.Format        == other.Format
                && this.SampleCount   == other.SampleCount
                && this.Usage         == other.Usage
                && this.EnableResolve == other.EnableResolve;

            public override readonly bool Equals(object? obj) =>
                obj is ColorAttachmentInfo colorAttachmentInfo && this.Equals(colorAttachmentInfo);

            public static bool operator ==(ColorAttachmentInfo left, ColorAttachmentInfo right) => left.Equals(right);
            public static bool operator !=(ColorAttachmentInfo left, ColorAttachmentInfo right) => !(left == right);
        }
    }
}
