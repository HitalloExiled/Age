using ThirdParty.Vulkan.Native.Handles;

namespace ThirdParty.Vulkan.Native.KHR;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkWin32SurfaceCreateInfoKHR.html">VkWin32SurfaceCreateInfoKHR</see>
/// </summary>
public unsafe struct VkWin32SurfaceCreateInfoKHR
{
    public readonly VkStructureType SType;

    public void*                        PNext;
    public VkWin32SurfaceCreateFlagsKHR Flags;
    public HINSTANCE                    Hinstance;
    public HWND                         Hwnd;

    public VkWin32SurfaceCreateInfoKHR() =>
        this.SType = VkStructureType.Win32SurfaceCreateInfoKHR;
}
