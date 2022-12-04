namespace Age.Drivers.GLES3.Storage;

internal class MeshStorage
{
    public static MeshStorage Singleton { get; } = new();

    public void UpdateDirtyMultimeshes() => // TODO - drivers\gles3\storage\mesh_storage.cpp[1519]
        throw new NotImplementedException();
}
