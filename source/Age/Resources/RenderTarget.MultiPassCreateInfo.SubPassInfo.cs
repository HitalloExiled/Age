namespace Age.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public struct SubPassInfo
        {
            public required int[] ColorAttachments;
            public required int?  DepthStencilAttachment;
        }
    }
}
