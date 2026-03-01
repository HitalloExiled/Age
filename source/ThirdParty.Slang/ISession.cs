namespace ThirdParty.Slang;

internal unsafe struct ISession
{
    internal struct VTable
    {
        public ISlangUnknown.VTable SlangUnknown;

        public delegate* unmanaged<ISession*, IGlobalSession*> GetGlobalSession;
        public void* LoadModule;
        public void* LoadModuleFromSource;
        public void* CreateCompositeComponentType;
        public void* SpecializeType;
        public void* GetTypeLayout;
        public void* GetContainerType;
        public void* GetDynamicType;
        public void* GetTypeRttiMangledName;
        public void* GetTypeConformanceWitnessMangledName;
        public void* GetTypeConformanceWitnessSequentialId;
        public void* CreateCompileRequest;
        public void* CreateTypeConformanceComponentType;
        public void* LoadModuleFromIrBlob;
        public void* GetLoadedModuleCount;
        public void* GetLoadedModule;
        public void* IsBinaryModuleUpToDate;
        public void* LoadModuleFromSourceString;
        public void* GetDynamicObjectRttiBytes;
        public void* LoadModuleInfoFromIrBlob;
    }

    internal VTable* Vtbl;
}
