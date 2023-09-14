using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Enums;

namespace Age.Vulkan.Native;

[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessageTypeFlagsEXT(VkDebugUtilsMessageTypeFlagBitsEXT Value) : IVkFlags
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(uint value) =>
        ((uint)this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDebugUtilsMessageTypeFlagsEXT value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageTypeFlagsEXT(VkDebugUtilsMessageTypeFlagBitsEXT value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageTypeFlagBitsEXT(VkDebugUtilsMessageTypeFlagsEXT value) => value.Value;
}
