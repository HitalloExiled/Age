namespace ThirdParty.Slang;

internal unsafe struct ISlangCastable
{
    internal struct VTable
    {
        internal ISlangUnknown.VTable SlangUnknown;

        internal delegate* unmanaged<void*, SlangUUID*, void*> CastAs;
    }

    internal VTable* Vtbl;
};
