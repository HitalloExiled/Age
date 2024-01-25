namespace ThirdParty.Vulkan.Native.KHR;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkWin32SurfaceCreateInfoKHR.html">VkWin32SurfaceCreateInfoKHR</see>
/// </summary>
public unsafe struct VkWin32SurfaceCreateInfoKHR
{
    public readonly VkStructureType sType;

    public void*                        pNext;
    public VkWin32SurfaceCreateFlagsKHR flags;
    public HINSTANCE                    hinstance;
    public HWND                         hwnd;

    public VkWin32SurfaceCreateInfoKHR() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR;
}
