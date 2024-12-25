#if !Windows
#define Windows
#endif

namespace ThirdParty.Slang;

internal static partial class PInvoke
{
#if Windows
    private const string PLATFORM_PATH = "runtimes/win-x64/native/slang.dll";
#endif
}
