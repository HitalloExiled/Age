using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask of <see cref="VkCullModeFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkCullModeFlags(VkCullModeFlagBits Value) : IVkFlags<VkCullModeFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkCullModeFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkCullModeFlags(VkCullModeFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkCullModeFlagBits(VkCullModeFlags value) => value.Value;
}
