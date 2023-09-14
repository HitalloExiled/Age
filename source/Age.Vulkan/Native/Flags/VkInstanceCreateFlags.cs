using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask of <see cref="VkInstanceCreateFlagBits"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkInstanceCreateFlags(VkInstanceCreateFlagBits Value) : IVkFlags<VkInstanceCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkInstanceCreateFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkInstanceCreateFlags(VkInstanceCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkInstanceCreateFlagBits(VkInstanceCreateFlags value) => value.Value;
}
