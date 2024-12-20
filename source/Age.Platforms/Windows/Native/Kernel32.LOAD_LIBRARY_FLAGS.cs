namespace Age.Platforms.Windows.Native;
internal static partial class Kernel32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryexw"></see>
    /// </summary>
    [Flags]
    public enum LOAD_LIBRARY_FLAGS : uint
    {
        DONT_RESOLVE_DLL_REFERENCES         = 0x00000001,
        LOAD_IGNORE_CODE_AUTHZ_LEVEL        = 0x00000010,
        LOAD_LIBRARY_AS_DATAFILE            = 0x00000002,
        LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE  = 0x00000040,
        LOAD_LIBRARY_AS_IMAGE_RESOURCE      = 0x00000020,
        LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200,
        LOAD_LIBRARY_SEARCH_DEFAULT_DIRS    = 0x00001000,
        LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR    = 0x00000100,
        LOAD_LIBRARY_SEARCH_SYSTEM32        = 0x00000800,
        LOAD_LIBRARY_SEARCH_USER_DIRS       = 0x00000400,
        LOAD_WITH_ALTERED_SEARCH_PATH       = 0x00000008,
        LOAD_LIBRARY_REQUIRE_SIGNED_TARGET  = 0x00000080,
        LOAD_LIBRARY_SAFE_CURRENT_DIRS      = 0x00002000
    }
}
