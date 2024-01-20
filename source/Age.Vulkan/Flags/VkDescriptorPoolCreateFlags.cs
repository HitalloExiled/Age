using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask of <see cref="VkDescriptorPoolCreateFlagBits"/>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkDescriptorPoolCreateFlags(VkDescriptorPoolCreateFlagBits Value) : IVkFlags<VkDescriptorPoolCreateFlagBits>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDescriptorPoolCreateFlagBits value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDescriptorPoolCreateFlags(VkDescriptorPoolCreateFlagBits value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDescriptorPoolCreateFlagBits(VkDescriptorPoolCreateFlags value) => value.Value;
}
