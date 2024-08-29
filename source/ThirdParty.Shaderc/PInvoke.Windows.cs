#if !Windows
#define Windows
#endif

namespace ThirdParty.Shaderc;

internal static partial class PInvoke
{
#if Windows
    private const string PLATFORM_PATH = "runtimes/win-x64/native/shaderc_shared.dll";
#endif
}
