#if !Windows
#define Windows
#endif

namespace ThirdParty.SpirvCross;

internal static partial class PInvoke
{
#if Windows
    private const string PLATFORM_PATH = "runtimes/win-x64/native/spirv-cross-c-shared.dll";
#endif
}
