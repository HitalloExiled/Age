using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags.KHR;

/// <summary>
/// Reserved for future use.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkWin32SurfaceCreateFlagsKHR(uint Value) : IVkFlags
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(uint value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkWin32SurfaceCreateFlagsKHR(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(VkWin32SurfaceCreateFlagsKHR value) => value.Value;
}
