using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask of <see cref="VkFormatFeatureFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkFormatFeatureFlags(VkFormatFeatureFlagBits Value) : IVkFlags<VkFormatFeatureFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkFormatFeatureFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkFormatFeatureFlags(VkFormatFeatureFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkFormatFeatureFlagBits(VkFormatFeatureFlags value) => value.Value;
}
