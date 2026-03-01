namespace ThirdParty.Slang;

internal unsafe struct ISlangFileSystem
{
    internal struct VTable
    {
        public ISlangCastable.VTable SlangCastable;

        public delegate* unmanaged<void*, byte*, ISlangBlob**, SlangResult> LoadFile;
    }

    internal VTable* LpVtbl;
}
