using Age.Servers.Rendering.Storage;

namespace Age.Drivers.GLES3.Storage;

internal class Utilities : RendererUtilities
{
    public static Utilities Singleton { get; } = new();

    public override void UpdateDirtyResources()
    {
        MaterialStorage.Singleton.UpdateGlobalShaderUniforms();
        MaterialStorage.Singleton.UpdateQueuedMaterials();
        MeshStorage.Singleton.UpdateDirtyMultimeshes();
	    TextureStorage.Singleton.UpdateTextureAtlas();
    }
}
