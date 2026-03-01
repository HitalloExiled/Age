namespace ThirdParty.Slang;

public unsafe struct IModule
{
    internal struct VTable
    {
        internal IComponentType.VTable ComponentType;

        internal delegate* unmanaged<IModule*, byte*, IEntryPoint**, SlangResult>      FindEntryPointByName;
        internal delegate* unmanaged<IModule*, SlangInt32>                             GetDefinedEntryPointCount;
        internal delegate* unmanaged<IModule*, SlangInt32, IEntryPoint**, SlangResult> GetDefinedEntryPoint;
        internal void* Serialize;
        internal void* WriteToFile;
        internal void* GetName;
        internal void* GetFilePath;
        internal void* GetUniqueIdentity;
        internal void* FindAndCheckEntryPoint;
        internal delegate* unmanaged<IModule*, SlangInt32>        GetDependencyFileCount;
        internal delegate* unmanaged<IModule*, SlangInt32, byte*> GetDependencyFilePath;
        internal void* GetModuleReflection;
        internal void* Disassemble;
    }

    internal VTable* Vtbl;
}
