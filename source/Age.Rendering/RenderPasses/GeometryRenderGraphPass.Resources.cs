using Age.Rendering.Resources;

namespace Age.Rendering.RenderPasses;

public partial class GeometryRenderGraphPass
{
    private struct Resources
    {
        public required Framebuffer Framebuffer;
        public required Image       ColorImage;
        public required Image       DepthImage;
        public required Image       ResolveImage;

        public readonly void Dispose()
        {
            this.Framebuffer.Dispose();
            this.ColorImage.Dispose();
            this.DepthImage.Dispose();
            this.ResolveImage.Dispose();
        }
    }
}
