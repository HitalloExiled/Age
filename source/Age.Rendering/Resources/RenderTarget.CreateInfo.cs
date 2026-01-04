using Age.Core.Collections;
using Age.Numerics;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public required Size<uint>                       Size;
        public required InlineList4<ColorAttachmentInfo> ColorAttachments;

        public DepthStencilAttachmentInfo? DepthStencilAttachment;
    }
}
