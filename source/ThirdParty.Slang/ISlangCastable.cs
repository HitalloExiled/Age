namespace ThirdParty.Slang;

internal unsafe struct ISlangCastable
{
    internal struct VTable
    {
        public ISlangUnknown.VTable SlangUnknown;

        public delegate* unmanaged<void*, SlangUUID*, void*> CastAs;
    }

    internal VTable* LpVtbl;
};
