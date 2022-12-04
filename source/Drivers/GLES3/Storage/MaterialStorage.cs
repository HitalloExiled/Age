namespace Age.Drivers.GLES3.Storage;

internal class MaterialStorage
{
    public static MaterialStorage Singleton { get; } = new();

    public void UpdateGlobalShaderUniforms() => // TODO - drivers\gles3\storage\material_storage.cpp[2374:2422]
        throw new NotImplementedException();

    public void UpdateQueuedMaterials() => // TODO - drivers\gles3\storage\material_storage.cpp[2625]
        throw new NotImplementedException();
}
