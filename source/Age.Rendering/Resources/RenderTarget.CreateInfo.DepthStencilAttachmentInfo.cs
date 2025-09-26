namespace Age.Rendering.Resources;

public sealed partial class RenderTarget
{
    public ref partial struct CreateInfo
    {
        public struct DepthStencilAttachmentInfo
        {
            internal Image? Image;

            public required TextureFormat Format;
            public required TextureAspect Aspect;

            public TextureUsage  Usage;
        }
    }
}
