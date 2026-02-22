namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public struct DepthStencilAttachmentInfo : IEquatable<DepthStencilAttachmentInfo>
        {
            internal Image? Image;

            public required ImageLayout   FinalLayout;
            public required TextureFormat Format;
            public required TextureAspect Aspect;

            public TextureUsage Usage;

            public override readonly int GetHashCode() =>
                HashCode.Combine(
                    this.FinalLayout,
                    this.Format,
                    this.Aspect,
                    this.Usage
                );

            public readonly bool Equals(DepthStencilAttachmentInfo other) =>
                this.FinalLayout == other.FinalLayout
                && this.Format   == other.Format
                && this.Aspect   == other.Aspect
                && this.Usage    == other.Usage;

            public override readonly bool Equals(object? obj) =>
                obj is DepthStencilAttachmentInfo depthStencilAttachmentInfo && this.Equals(depthStencilAttachmentInfo);

            public static bool operator ==(DepthStencilAttachmentInfo left, DepthStencilAttachmentInfo right) => left.Equals(right);
            public static bool operator !=(DepthStencilAttachmentInfo left, DepthStencilAttachmentInfo right) => !(left == right);
        }
    }
}
