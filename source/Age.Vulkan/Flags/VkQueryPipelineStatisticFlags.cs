using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkQueryPipelineStatisticFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkQueryPipelineStatisticFlags(VkQueryPipelineStatisticFlagBits Value) : IVkFlags<VkQueryPipelineStatisticFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkQueryPipelineStatisticFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkQueryPipelineStatisticFlags(VkQueryPipelineStatisticFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkQueryPipelineStatisticFlagBits(VkQueryPipelineStatisticFlags value) => value.Value;
}
