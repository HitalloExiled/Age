using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native;

/// <summary>
/// Vulkan device memory size and offsets.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDeviceSize(ulong Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDeviceSize(ulong value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ulong(VkDeviceSize value) => value.Value;
}
