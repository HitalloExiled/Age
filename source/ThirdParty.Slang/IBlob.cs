namespace ThirdParty.Slang;

public unsafe struct IBlob
{
    internal struct VTable
    {
        internal ISlangUnknown.VTable SlangUnknown;

        internal delegate* unmanaged<void*, void*> GetBufferPointer;
        internal delegate* unmanaged<void*, size_t> GetBufferSize;
    }

    internal VTable* Vtbl;
}
