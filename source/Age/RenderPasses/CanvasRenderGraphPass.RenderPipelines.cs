using Age.Rendering.Resources;
using Age.Shaders;

namespace Age.RenderPasses;

public abstract partial class CanvasRenderGraphPass
{
    protected struct RenderPipelines(CanvasShader shader, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, bool enabled, bool ignoreStencil)
    {
        #region 8-bytes
        public IndexBuffer  IndexBuffer   = indexBuffer;
        public CanvasShader Shader        = shader;
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
