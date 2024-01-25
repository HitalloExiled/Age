using System.Runtime.InteropServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Enums;
using Age.Vulkan.Types;
using Age.Vulkan.Types.KHR;

using static Age.Core.Interop.PointerHelper;

namespace Age.Vulkan.Extensions.KHR;

/// <remarks>Provided by VK_KHR_win32_surface</remarks>
public unsafe class VkKhrWin32SurfaceExtension(Vk vk, VkInstance instance) : IVkInstanceExtension<VkKhrWin32SurfaceExtension>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate VkResult VkCreateWin32SurfaceKHR(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface);

    public static string Name { get; } = "VK_KHR_win32_surface";

    private readonly VkCreateWin32SurfaceKHR vkCreateWin32SurfaceKHR = vk.GetInstanceProcAddr<VkCreateWin32SurfaceKHR>(Name, instance, "vkCreateWin32SurfaceKHR");

    public static VkKhrWin32SurfaceExtension Create(Vk vk, VkInstance instance) =>
        new(vk, instance);

    /// <summary>
    /// Create a VkSurfaceKHR object for an Win32 native window.
    /// </summary>
    /// <param name="instance">The instance to associate the surface with.</param>
    /// <param name="pCreateInfo">A pointer to a <see cref="Win32SurfaceCreateInfo"/> structure containing parameters affecting the creation of the surface object.</param>
    /// <param name="pAllocator">The allocator used for host memory allocated for the surface object when there is no more specific allocator available (see <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#memory-allocation">Memory Allocation</see>).</param>
    /// <param name="pSurface">A pointer to a <see cref="VkSurfaceKHR"/> handle in which the created surface object is returned.</param>
    /// <returns></returns>
    public VkResult CreateWin32Surface(VkInstance instance, VkWin32SurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface) =>
        this.vkCreateWin32SurfaceKHR.Invoke(instance, pCreateInfo, pAllocator, pSurface);

    public VkResult CreateWin32Surface(VkInstance instance, in VkWin32SurfaceCreateInfoKHR createInfo, in VkAllocationCallbacks allocator, out VkSurfaceKHR surface)
    {
        fixed (VkWin32SurfaceCreateInfoKHR* pCreateInfo = &createInfo)
        fixed (VkAllocationCallbacks*       pAllocator  = &allocator)
        fixed (VkSurfaceKHR*                pSurface    = &surface)
        {
            return this.vkCreateWin32SurfaceKHR.Invoke(instance, pCreateInfo, NullIfDefault(allocator, pAllocator), pSurface);
        }
    }
}
