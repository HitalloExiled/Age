namespace ThirdParty.Slang;

public unsafe struct ISlangFileSystem
{
    internal struct VTable
    {
        internal ISlangCastable.VTable SlangCastable;

        internal delegate* unmanaged<void*, byte*, ISlangBlob**, SlangResult> LoadFile;
    }

    internal VTable* Vtbl;
}
