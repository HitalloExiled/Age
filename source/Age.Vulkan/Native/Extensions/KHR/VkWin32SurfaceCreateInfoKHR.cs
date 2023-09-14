
using Age.Vulkan.Native.Extensions.EXT.VkFlags;
using HINSTANCE = nint;
using HWND      = nint;

namespace Age.Vulkan.Native.Extensions.KHR;

/// <summary>
/// Structure specifying parameters of a newly created Win32 surface object;
/// </summary>
public unsafe struct VkWin32SurfaceCreateInfoKHR
{
    /// <summary>
    /// a VkStructureType value identifying this structure.
    /// </summary>
    public VkStructureType sType;

    /// <summary>
    /// NULL or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// reserved for future use.
    /// </summary>
    public VkWin32SurfaceCreateFlagsKHR flags;

    /// <summary>
    /// the Win32 <see cref="HINSTANCE"/> for the window to associate the surface with.
    /// </summary>
    public HINSTANCE hinstance;

    /// <summary>
    /// the Win32 <see cref="HWND"/> for the window to associate the surface with.
    /// </summary>
    public HWND hwnd;
}
