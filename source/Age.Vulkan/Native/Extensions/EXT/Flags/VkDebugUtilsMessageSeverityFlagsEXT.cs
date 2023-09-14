using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Extensions.EXT.Enums;

namespace Age.Vulkan.Native.Extensions.EXT.Flags;

/// <summary>
/// Bitmask of <see cref="VkDebugUtilsMessageSeverityFlagBitsEXT"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessageSeverityFlagsEXT(VkDebugUtilsMessageSeverityFlagBitsEXT Value) : IVkFlags<VkDebugUtilsMessageSeverityFlagBitsEXT>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDebugUtilsMessageSeverityFlagBitsEXT value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageSeverityFlagsEXT(VkDebugUtilsMessageSeverityFlagBitsEXT value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageSeverityFlagBitsEXT(VkDebugUtilsMessageSeverityFlagsEXT value) => value.Value;
}
