using Age.Rendering.Resources;

namespace Age.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass
{
    protected struct RenderResources(Shader shader, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, bool enabled)
    {
        public Shader       Shader       = shader;
        public VertexBuffer VertexBuffer = vertexBuffer;
        public IndexBuffer  IndexBuffer  = indexBuffer;
        public bool         Enabled      = enabled;

        public readonly void Dispose()
        {
            this.Shader.Dispose();
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }
}
