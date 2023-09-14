using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native.Types.KHR;

/// <summary>
/// <para>Opaque handle to a surface object.</para>
/// <para>The VK_KHR_surface extension declares the <see cref="VkSurfaceKHR"/> object, and provides a function for destroying <see cref="VkSurfaceKHR"/> objects. Separate platform-specific extensions each provide a function for creating a <see cref="VkSurfaceKHR"/> object for the respective platform. From the application’s perspective this is an opaque handle, just like the handles of other Vulkan objects.</para>
/// <remarks>On certain platforms, the Vulkan loader and ICDs may have conventions that treat the handle as a pointer to a structure containing the platform-specific information about the surface. This will be described in the documentation for the loader-ICD interface, and in the vk_icd.h header file of the LoaderAndTools source-code repository. This does not affect the loader-layer interface; layers may wrap <see cref="VkSurfaceKHR"/> objects.</remarks>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkSurfaceKHR(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSurfaceKHR(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkSurfaceKHR value) => value.Value;
}
