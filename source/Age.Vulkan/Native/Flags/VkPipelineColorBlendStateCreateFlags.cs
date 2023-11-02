using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask of <see cref="VkPipelineColorBlendStateCreateFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkPipelineColorBlendStateCreateFlags(VkPipelineColorBlendStateCreateFlagBits Value) : IVkFlags<VkPipelineColorBlendStateCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkPipelineColorBlendStateCreateFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineColorBlendStateCreateFlags(VkPipelineColorBlendStateCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineColorBlendStateCreateFlagBits(VkPipelineColorBlendStateCreateFlags value) => value.Value;
}
