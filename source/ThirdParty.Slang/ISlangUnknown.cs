namespace ThirdParty.Slang;

internal unsafe struct ISlangUnknown
{
    internal struct VTable
    {
        public delegate* unmanaged<void*, SlangUUID, void**, SlangResult> QueryInterface;
        public delegate* unmanaged<void*, uint32_t>                       AddRef;
        public delegate* unmanaged<void*, uint32_t>                       Release;
    }
}
