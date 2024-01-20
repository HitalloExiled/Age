using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkPipelineCreateFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkPipelineCreateFlags(VkPipelineCreateFlagBits Value) : IVkFlags<VkPipelineCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkPipelineCreateFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineCreateFlags(VkPipelineCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineCreateFlagBits(VkPipelineCreateFlags value) => value.Value;
}
