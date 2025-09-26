namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct MultiPassCreateInfo
    {
        public struct SubPassDependency
        {
            public int  SrcSubpass;
            public int  DstSubpass;
            public uint SrcStageMask;
            public uint DstStageMask;
            public uint SrcAccessMask;
            public uint DstAccessMask;
        }
    }
}
