using Age.Core.Math;

using RealT = System.Single;
using RS    = Age.Servers.RenderingServer;

namespace Age.Servers.Rendering.Storage;

internal abstract class RenderSceneBuffers
{
    public abstract void Configure(Guid renderTarget, Vector2<int> internalSize, Vector2<int> vector2D, RealT fsrSharpness, double textureMipmapBias, RS.ViewportMSAA mSAA3d, RS.ViewportScreenSpaceAA screenSpaceAA, bool useTaa, bool useDebanding, int viewCount);
}
