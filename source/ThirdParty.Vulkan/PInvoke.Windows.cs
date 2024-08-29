#if !Windows
#define Windows
#endif

namespace ThirdParty.Vulkan;

internal static partial class PInvoke
{
#if Windows
    private const string PLATFORM_PATH = "vulkan-1.dll";
#endif
}
