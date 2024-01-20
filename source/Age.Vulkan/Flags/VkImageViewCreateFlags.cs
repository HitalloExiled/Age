using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkImageViewCreateFlagBits"/>
/// Reserved for future use.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkImageViewCreateFlags(VkImageViewCreateFlagBits Value) : IVkFlags<VkImageViewCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkImageViewCreateFlagBits value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkImageViewCreateFlags(VkImageViewCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkImageViewCreateFlagBits(VkImageViewCreateFlags value) => value.Value;
}
