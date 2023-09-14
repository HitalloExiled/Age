using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask of <see cref="VkDeviceQueueCreateFlagBits"/>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDeviceQueueCreateFlags(VkDeviceQueueCreateFlagBits Value) : IVkFlags<VkDeviceQueueCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDeviceQueueCreateFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDeviceQueueCreateFlags(VkDeviceQueueCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDeviceQueueCreateFlagBits(VkDeviceQueueCreateFlags value) => value.Value;
}
