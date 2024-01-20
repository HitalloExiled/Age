using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkPipelineShaderStageCreateFlagBits"/>.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkPipelineShaderStageCreateFlags(VkPipelineShaderStageCreateFlagBits Value) : IVkFlags<VkPipelineShaderStageCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkPipelineShaderStageCreateFlagBits value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineShaderStageCreateFlags(VkPipelineShaderStageCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineShaderStageCreateFlagBits(VkPipelineShaderStageCreateFlags value) => value.Value;
}
