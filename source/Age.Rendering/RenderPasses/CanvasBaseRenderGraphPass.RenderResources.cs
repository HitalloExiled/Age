using Age.Rendering.Resources;

namespace Age.Rendering.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass
{
    protected struct RenderResources(Shader shader, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
    {
        public Shader       Shader       = shader;
        public VertexBuffer VertexBuffer = vertexBuffer;
        public IndexBuffer  IndexBuffer  = indexBuffer;

        public readonly void Dispose()
        {
            this.Shader.Dispose();
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }
}
