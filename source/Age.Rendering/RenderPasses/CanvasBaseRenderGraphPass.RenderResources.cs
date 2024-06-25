using Age.Rendering.Resources;

namespace Age.Rendering.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass
{
    protected struct RenderResources(Pipeline pipeline, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, bool enabled)
    {
        public Pipeline     Pipeline     = pipeline;
        public VertexBuffer VertexBuffer = vertexBuffer;
        public IndexBuffer  IndexBuffer  = indexBuffer;
        public bool         Enabled      = enabled;

        public readonly void Dispose()
        {
            this.Pipeline.Dispose();
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }
}
