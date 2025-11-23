using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public struct DepthStencilAttachmentInfo
        {
            internal Image? Image;

            public required ImageLayout   FinalLayout;
            public required TextureFormat Format;
            public required TextureAspect Aspect;

            public TextureUsage Usage;

            public override readonly int GetHashCode()
            {
                var hashcode = new HashCode();

                hashcode.Add(this.Format);
                hashcode.Add(this.Aspect);
                hashcode.Add(this.Usage);

                return hashcode.ToHashCode();
            }
        }
    }
}
