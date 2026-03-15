namespace ThirdParty.Slang;

public unsafe struct IComponentType
{
    internal struct VTable
    {
        internal ISlangUnknown.VTable SlangUnknown;

        internal void* GetSession;
        internal delegate* unmanaged<IComponentType*, SlangInt, IBlob**, Handle<ShaderReflection>> GetLayout;
        internal void* GetSpecializationParamCount;
        internal delegate* unmanaged<IComponentType*, SlangInt, SlangInt, IBlob**, IBlob**, SlangResult> GetEntryPointCode;
        internal void* GetResultAsFileSystem;
        internal void* GetEntryPointHash;
        internal void* Specialize;
        internal delegate* unmanaged<IComponentType*, IComponentType**, IBlob**, SlangResult> Link;
        internal void* GetEntryPointHostCallable;
        internal void* RenameEntryPoint;
        internal void* LinkWithOptions;
        internal void* GetTargetCode;
        internal void* GetTargetMetadata;
        internal void* GetEntryPointMetadata;
    }

    internal VTable* Vtbl;
}
