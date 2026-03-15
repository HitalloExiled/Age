namespace ThirdParty.Slang;

public unsafe struct IEntryPoint
{
    internal struct VTable
    {
        internal IComponentType.VTable ComponentType;

        internal void* GetFunctionReflection;
    }

    internal VTable* Vtbl;
}
