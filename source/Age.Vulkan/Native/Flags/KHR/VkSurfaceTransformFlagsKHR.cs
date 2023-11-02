using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native.Flags.KHR;

/// <summary>
/// Bitmask of <see cref="VkSurfaceTransformFlagBitsKHR"/>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkSurfaceTransformFlagsKHR(VkSurfaceTransformFlagBitsKHR Value) : IVkFlags<VkSurfaceTransformFlagBitsKHR>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkSurfaceTransformFlagBitsKHR value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSurfaceTransformFlagsKHR(VkSurfaceTransformFlagBitsKHR value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSurfaceTransformFlagBitsKHR(VkSurfaceTransformFlagsKHR value) => value.Value;
}
