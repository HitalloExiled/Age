using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native;

/// <summary>
/// Opaque handle to a device object.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDevice(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDevice(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkDevice value) => value.Value;
}
