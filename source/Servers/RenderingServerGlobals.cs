using Age.Servers.Rendering;
using Age.Servers.Rendering.Storage;

namespace Age.Servers;

internal static class RenderingServerGlobals
{
    public static Rasterizer         Rasterizer    => Rasterizer.Singleton;
    public static RendererSceneCull  Scene         => RendererSceneCull.Singleton;
    public static TextureStorage    TextureStorage => Drivers.GLES3.TextureStorage.Singleton;
    public static RendererUtilities Utilities      => Drivers.GLES3.Storage.Utilities.Singleton;
    public static RendererViewport   Viewport      => RendererViewport.Singleton;
}
