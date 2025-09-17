using System.Runtime.InteropServices;
using static Age.Resources.RenderTarget.CreateInfo;

namespace Age.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        [StructLayout(LayoutKind.Explicit)]
        public readonly partial struct AttachmentInfo
        {
#if TARGET_64BIT
            private const int OFFSET = 8;
#else
            private const int OFFSET = 4;
#endif

            [FieldOffset(0)]
            private readonly AttachmentInfoKind kind;

            [FieldOffset(OFFSET)]
            private readonly Union union;

            public AttachmentInfo(in ColorAttachmentInfo colorAttachmentInfo)
            {
                this.kind  = AttachmentInfoKind.Color;
                this.union = new() { ColorAttachmentInfo = colorAttachmentInfo };
            }

            public AttachmentInfo(in DepthStencilAttachmentInfo depthStencilAttachmentInfo)
            {
                this.kind  = AttachmentInfoKind.DepthStencil;
                this.union = new() { DepthStencilAttachmentInfo = depthStencilAttachmentInfo };
            }

            public bool TryGetColorAttachment(out ColorAttachmentInfo colorAttachmentInfo)
            {
                if (this.kind == AttachmentInfoKind.Color)
                {
                    colorAttachmentInfo = this.union.ColorAttachmentInfo;

                    return true;
                }

                colorAttachmentInfo = default;
                return false;
            }

            public bool TryGetDepthStencilAttachment(out DepthStencilAttachmentInfo depthStencilAttachmentInfo)
            {
                if (this.kind == AttachmentInfoKind.DepthStencil)
                {
                    depthStencilAttachmentInfo = this.union.DepthStencilAttachmentInfo;

                    return true;
                }

                depthStencilAttachmentInfo = default;
                return false;
            }

            public override string ToString() =>
                this.kind switch
                {
                    AttachmentInfoKind.Color        => nameof(ColorAttachmentInfo),
                    AttachmentInfoKind.DepthStencil => nameof(DepthStencilAttachmentInfo),
                    _ => "InvalidAttachment"
                };

            public static implicit operator AttachmentInfo(in ColorAttachmentInfo colorAttachmentInfo)               => new(colorAttachmentInfo);
            public static implicit operator AttachmentInfo(in DepthStencilAttachmentInfo depthStencilAttachmentInfo) => new(depthStencilAttachmentInfo);

            public static implicit operator ColorAttachmentInfo(in AttachmentInfo attachmentInfo)        => attachmentInfo.union.ColorAttachmentInfo;
            public static implicit operator DepthStencilAttachmentInfo(in AttachmentInfo attachmentInfo) => attachmentInfo.union.DepthStencilAttachmentInfo;
        }
    }
}
