using Age.Numerics;

namespace Age.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public required Size<uint>                        Size;
        public required ReadOnlySpan<ColorAttachmentInfo> ColorAttachments;

        public DepthStencilAttachmentInfo? DepthStencilAttachment;
    }
}
