using System.Runtime.InteropServices;
using static Age.Resources.RenderTarget.CreateInfo;

namespace Age.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public readonly partial struct AttachmentInfo
        {
            [StructLayout(LayoutKind.Explicit)]
            private struct Union
            {
                [FieldOffset(0)]
                public ColorAttachmentInfo ColorAttachmentInfo;

                [FieldOffset(0)]
                public DepthStencilAttachmentInfo DepthStencilAttachmentInfo;
            }
        }
    }
}
