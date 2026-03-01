namespace ThirdParty.Vulkan;

internal static partial class PInvoke
{
#if WINDOWS
    private const string PLATFORM_PATH = "vulkan-1";
#elif LINUX
    private const string PLATFORM_PATH = "libvulkan";
#endif
}
