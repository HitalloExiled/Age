using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums.KHR;

namespace Age.Vulkan.Native.Flags.KHR;

/// <summary>
/// Bitmask of <see cref="VkSwapchainCreateFlagBitsKHR"/>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkSwapchainCreateFlagsKHR(VkSwapchainCreateFlagBitsKHR Value) : IVkFlags<VkSwapchainCreateFlagBitsKHR>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkSwapchainCreateFlagBitsKHR value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSwapchainCreateFlagsKHR(VkSwapchainCreateFlagBitsKHR value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSwapchainCreateFlagBitsKHR(VkSwapchainCreateFlagsKHR value) => value.Value;
}
