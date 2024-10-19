using Age.Rendering.Resources;

namespace Age.RenderPasses;

public abstract partial class CanvasBaseRenderGraphPass
{
    protected struct RenderPipelines(Shader shader, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, bool enabled, bool ignoreStencil)
    {
        #region 8-bytes
        public IndexBuffer  IndexBuffer   = indexBuffer;
        public Shader       Shader        = shader;
        public VertexBuffer VertexBuffer  = vertexBuffer;
        #endregion

        #region 1-byte
        public bool Enabled       = enabled;
        public bool IgnoreStencil = ignoreStencil;
        #endregion

        public readonly void Dispose()
        {
            this.Shader.Dispose();
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }
}
