namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public readonly partial struct AttachmentInfo
        {
            private class DebugView(AttachmentInfo attachment)
            {
                public object? Value => attachment.kind switch
                {
                    AttachmentInfoKind.Color        => attachment.union.ColorAttachmentInfo,
                    AttachmentInfoKind.DepthStencil => attachment.union.DepthStencilAttachmentInfo,
                    _ => null
                };
            }
        }
    }
}
