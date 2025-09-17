using Age.Rendering.Resources;

namespace Age.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public struct ColorAttachmentInfo
        {
            internal Image? Image;

            public required TextureFormat Format;
            public required SampleCount   SampleCount;

            public TextureUsage Usage;
            public bool         EnableResolve;

            internal static ColorAttachmentInfo From(Image image) =>
                new()
                {
                    Image       = image,
                    Format      = (TextureFormat)image.Format,
                    SampleCount = (SampleCount)image.Samples,
                    Usage       = (TextureUsage)image.Usage,
                };
        }
    }
}
