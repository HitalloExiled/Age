namespace ThirdParty.Slang;

public unsafe struct ISlangFileSystem
{
    internal struct VTable
    {
        internal ISlangCastable.VTable SlangCastable;

        internal delegate* unmanaged<void*, byte*, IBlob**, SlangResult> LoadFile;
    }

    internal VTable* Vtbl;
}

public unsafe abstract class SlangCastable(ISlangCastable* handle, bool ownsHandler) : SlangUnknown((ISlangUnknown*)handle, ownsHandler)
{
}
