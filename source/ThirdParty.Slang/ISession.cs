namespace ThirdParty.Slang;

public unsafe struct ISession
{
    internal struct VTable
    {
        internal ISlangUnknown.VTable SlangUnknown;

        internal delegate* unmanaged<ISession*, IGlobalSession*> GetGlobalSession;
        internal delegate* unmanaged<ISession*, byte*, IBlob**, IModule*> LoadModule;
        internal delegate* unmanaged<ISession*, byte*, byte*, IBlob*, IBlob**, IModule*> LoadModuleFromSource;
        internal delegate* unmanaged<ISession*, IComponentType**, SlangInt, IComponentType**, IBlob**, SlangResult> CreateCompositeComponentType;
        internal void* SpecializeType;
        internal void* GetTypeLayout;
        internal void* GetContainerType;
        internal void* GetDynamicType;
        internal void* GetTypeRttiMangledName;
        internal void* GetTypeConformanceWitnessMangledName;
        internal void* GetTypeConformanceWitnessSequentialId;
        internal void* CreateCompileRequest;
        internal void* CreateTypeConformanceComponentType;
        internal void* LoadModuleFromIrBlob;
        internal void* GetLoadedModuleCount;
        internal void* GetLoadedModule;
        internal void* IsBinaryModuleUpToDate;
        internal delegate* unmanaged<ISession*, byte*, byte*, byte*, IBlob**, IModule*> LoadModuleFromSourceString;
        internal void* GetDynamicObjectRttiBytes;
        internal void* LoadModuleInfoFromIrBlob;
    }

    internal VTable* Vtbl;
}
