using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Reserved for future use
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDeviceCreateFlags(uint Value) : IVkFlags
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(uint value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDeviceCreateFlags(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(VkDeviceCreateFlags value) => value.Value;
}