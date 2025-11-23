using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public struct ColorAttachmentInfo
        {
            internal Image? Image;

            public required ImageLayout   FinalLayout;
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
                    FinalLayout = (ImageLayout)image.FinalLayout,
                };

            public override readonly int GetHashCode()
            {
                var hashcode = new HashCode();

                hashcode.Add(this.Format);
                hashcode.Add(this.SampleCount);
                hashcode.Add(this.Usage);
                hashcode.Add(this.EnableResolve);

                return hashcode.ToHashCode();
            }
        }
    }
}
