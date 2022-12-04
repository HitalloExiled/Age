using Age.Core.Math;
using Age.Servers.Rendering.Storage;

using RealT = System.Single;
using RS    = Age.Servers.RenderingServer;

namespace Age.Drivers.GLES3.Storage;

internal class RenderSceneBuffersGLES3 : RenderSceneBuffers
{
    public int  Height        { get; set; }
    public Guid RenderTarget  { get; set; }
    public int  ViewCount     { get; set; }
    public int  Width         { get; set; }
    public bool IsTransparent { get; set; }

    public override void Configure(
        Guid                                  renderTargetId,
        Vector2<int>             internalSize,
        Vector2<int>             targetSize,
        RealT                    fsrSharpness,
        double                   textureMipmapBias,
        RS.ViewportMSAA          mSAA3d,
        RS.ViewportScreenSpaceAA screenSpaceAA,
        bool                     useTaa,
        bool                     useDebanding,
        int                      viewCount
    )
    {
        var textureStorage = TextureStorage.Singleton;

        this.Width        = targetSize.X;
        this.Height       = targetSize.Y;
        this.RenderTarget = renderTargetId;
        this.ViewCount    = viewCount;

        // this.FreeRenderBufferData(); Do nothing

        var renderTarget = textureStorage.GetRenderTarget(renderTargetId);

        this.IsTransparent = renderTarget.IsTransparent;
    }
}
