using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native;

[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessageSeverityFlagsEXT(VkDebugUtilsMessageSeverityFlagBitsEXT Value) : IVkFlags
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(uint value) =>
        ((uint)this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDebugUtilsMessageSeverityFlagsEXT value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageSeverityFlagsEXT(VkDebugUtilsMessageSeverityFlagBitsEXT value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageSeverityFlagBitsEXT(VkDebugUtilsMessageSeverityFlagsEXT value) => value.Value;
}
