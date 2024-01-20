using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Reserved for future use
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkPipelineMultisampleStateCreateFlags(uint Value) : IVkFlags
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(uint value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPipelineMultisampleStateCreateFlags(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(VkPipelineMultisampleStateCreateFlags value) => value.Value;
}
