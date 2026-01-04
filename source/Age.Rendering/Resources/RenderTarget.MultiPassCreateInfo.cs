using Age.Core.Collections;
using Age.Numerics;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public required Size<uint>                  Size;
        public required InlineList4<AttachmentInfo> Attachments;
        public required InlineList4<SubPassInfo>    Passes;

        public InlineList4<SubPassDependency> Dependencies;
    }
}
