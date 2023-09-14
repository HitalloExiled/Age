using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums.KHR;

namespace Age.Vulkan.Native.Flags.KHR;

/// <summary>
/// Bitmask of <see cref="VkCompositeAlphaFlagBitsKHR"/>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkCompositeAlphaFlagsKHR(VkCompositeAlphaFlagBitsKHR Value) : IVkFlags<VkCompositeAlphaFlagBitsKHR>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkCompositeAlphaFlagBitsKHR value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkCompositeAlphaFlagsKHR(VkCompositeAlphaFlagBitsKHR value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkCompositeAlphaFlagBitsKHR(VkCompositeAlphaFlagsKHR value) => value.Value;
}
