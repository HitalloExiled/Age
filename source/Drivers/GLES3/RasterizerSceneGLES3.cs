using Age.Drivers.GLES3.Storage;
using Age.Servers.Rendering.Storage;

namespace Age.Drivers.GLES3;

internal class RasterizerSceneGLES3
{
    #pragma warning disable CA1822
    public RenderSceneBuffers RenderBuffersCreate() =>
        new RenderSceneBuffersGLES3();
}
