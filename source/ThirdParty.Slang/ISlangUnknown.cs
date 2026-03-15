namespace ThirdParty.Slang;

public unsafe struct ISlangUnknown
{
    internal struct VTable
    {
        internal delegate* unmanaged<void*, SlangUUID, void**, SlangResult> QueryInterface;
        internal delegate* unmanaged<void*, uint32_t>                       AddRef;
        internal delegate* unmanaged<void*, uint32_t>                       Release;
    }

    internal VTable* Vtbl;
}
