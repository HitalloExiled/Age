using System.Diagnostics;
using System.Runtime.InteropServices;
using static Age.Rendering.Resources.RenderTarget.CreateInfo;

namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        [StructLayout(LayoutKind.Explicit)]
        [DebuggerTypeProxy(typeof(DebugView))]
        public readonly partial struct AttachmentInfo : IEquatable<AttachmentInfo>
        {
#if TARGET_32BIT
            private const int OFFSET = 4;
#else
            private const int OFFSET = 8;
#endif

            [FieldOffset(0)]
            private readonly AttachmentInfoKind kind;

            [FieldOffset(OFFSET)]
            private readonly Union union;

            private AttachmentInfo(in ColorAttachmentInfo colorAttachmentInfo)
            {
                this.kind  = AttachmentInfoKind.Color;
                this.union = new() { ColorAttachmentInfo = colorAttachmentInfo };
            }

            private AttachmentInfo(in DepthStencilAttachmentInfo depthStencilAttachmentInfo)
            {
                this.kind  = AttachmentInfoKind.DepthStencil;
                this.union = new() { DepthStencilAttachmentInfo = depthStencilAttachmentInfo };
            }

            public bool Equals(AttachmentInfo other) =>
                this.kind == other.kind && this.kind switch
                {
                    AttachmentInfoKind.Color        => this.union.ColorAttachmentInfo.Equals(other.union.ColorAttachmentInfo),
                    AttachmentInfoKind.DepthStencil => this.union.DepthStencilAttachmentInfo.Equals(other.union.DepthStencilAttachmentInfo),
                    _ => false
                };

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

            public readonly override int GetHashCode() =>
                this.kind switch
                {
                    AttachmentInfoKind.Color        => this.union.ColorAttachmentInfo.GetHashCode(),
                    AttachmentInfoKind.DepthStencil => this.union.DepthStencilAttachmentInfo.GetHashCode(),
                    _ => 0,
                };

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
