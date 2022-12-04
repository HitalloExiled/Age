using Age.Core.Math;

namespace Age.Servers.Rendering;

internal class RendererSceneOcclusionCull
{
    public static RendererSceneOcclusionCull Singleton { get; } = new();

    private static void PrintWarning() => throw new NotImplementedException();

    #pragma warning disable IDE0060, CA1822
    public void BufferSetSize(Guid id, Vector2<int> newSize) => PrintWarning();
    public void BufferUpdate(Guid viewportId, Transform3D<float> mainTransform, Projection<float> mainProjection, bool isOrthogonal)
    {
        // Compatibility?
    }
}
