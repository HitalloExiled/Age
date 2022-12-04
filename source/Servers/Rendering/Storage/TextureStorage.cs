using Age.Core.Math;

namespace Age.Servers.Rendering.Storage;

internal abstract class TextureStorage
{
    public abstract Color DefaultClearColor { get; set; }

    public abstract void RenderTargetRequestClear(Guid renderTarget, Color color);
    public abstract void RenderTargetSetAsUnused(Guid guid);
    public abstract void RenderTargetSetOverride(Guid renderTarget, Guid colorTexture, Guid depthTexture, Guid velocityTexture);
    public abstract void UpdateTextureAtlas();
}
