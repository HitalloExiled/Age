using Age.Core.Math;
using Age.Servers.Rendering.Storage;
using RealT = System.Single;

using RS = Age.Servers.RenderingServer;

namespace Age.Servers;

internal record Viewport(Guid Id, Guid RenderTarget)
{
    public Guid                      Camera               { get; set; }
    public RS.ViewportClearMode      ClearMode            { get; set; }
    public bool                      Disable2D            { get; set; }
    public bool                      Disable3D            { get; set; }
    public bool                      DisableEnvironment   { get; set; }
    public bool                      FsrEnabled           { get; set; }
    public RealT                     FsrSharpness         { get; set; }
    public Vector2<int>              InternalSize         { get; set; }
    public int                       LastPass             { get; set; }
    public RealT                     MeshLodThreshold     { get; set; }
    public RS.ViewportMSAA           MSAA3d               { get; set; }
    public bool                      OcclusionBufferDirty { get; set; }
    public RenderSceneBuffers?       RenderBuffers        { get; set; }
    public RenderInfo                RenderInfo           { get; set; } = new();
    public RS.ViewportScaling3DMode  Scaling3dMode        { get; set; }
    public RealT                     Scaling3dScale       { get; set; }
    public Guid                      Scenario             { get; set; }
    public RS.ViewportScreenSpaceAA  ScreenSpaceAA        { get; set; }
    public Guid                      ShadowAtlas          { get; set; }
    public Vector2<int>              Size                 { get; set; }
    public RealT                     TextureMipmapBias    { get; set; }
    public bool                      TransparentBg        { get; set; }
    public bool                      UseDebanding         { get; set; }
    public bool                      UseOcclusionCulling  { get; set; }
    public bool                      UseTaa               { get; set; }
    public int                       ViewCount            { get; set; }
}

internal record RenderInfo
{
    public int[,] Info { get; } = new int[RS.VIEWPORT_RENDER_INFO_TYPE_MAX, RS.VIEWPORT_RENDER_INFO_MAX];
}
