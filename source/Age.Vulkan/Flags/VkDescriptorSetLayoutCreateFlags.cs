using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkDescriptorSetLayoutCreateFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkDescriptorSetLayoutCreateFlags(VkDescriptorSetLayoutCreateFlagBits Value) : IVkFlags<VkDescriptorSetLayoutCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDescriptorSetLayoutCreateFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDescriptorSetLayoutCreateFlags(VkDescriptorSetLayoutCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDescriptorSetLayoutCreateFlagBits(VkDescriptorSetLayoutCreateFlags value) => value.Value;
}
