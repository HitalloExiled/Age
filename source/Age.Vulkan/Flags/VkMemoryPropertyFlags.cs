using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkMemoryPropertyFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkMemoryPropertyFlags(VkMemoryPropertyFlagBits Value) : IVkFlags<VkMemoryPropertyFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkMemoryPropertyFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkMemoryPropertyFlags(VkMemoryPropertyFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkMemoryPropertyFlagBits(VkMemoryPropertyFlags value) => value.Value;
}
