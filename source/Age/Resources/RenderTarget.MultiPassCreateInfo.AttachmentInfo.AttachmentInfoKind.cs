namespace Age.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public readonly partial struct AttachmentInfo
        {
            private enum AttachmentInfoKind
            {
                None,
                Color,
                DepthStencil,
            }
        }
    }
}
