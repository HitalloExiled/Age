using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkPipelineDepthStencilStateCreateFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkPipelineDepthStencilStateCreateFlags(VkPipelineDepthStencilStateCreateFlagBits Value) : IVkFlags<VkPipelineDepthStencilStateCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkPipelineDepthStencilStateCreateFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineDepthStencilStateCreateFlags(VkPipelineDepthStencilStateCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineDepthStencilStateCreateFlagBits(VkPipelineDepthStencilStateCreateFlags value) => value.Value;
}
