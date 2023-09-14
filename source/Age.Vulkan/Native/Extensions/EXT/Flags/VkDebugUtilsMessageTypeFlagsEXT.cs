using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;
using Age.Vulkan.Native.Extensions.EXT.Enums;

namespace Age.Vulkan.Native.Extensions.EXT.Flags;

/// <summary>
/// Bitmask of <see cref="VkDebugUtilsMessageTypeFlagBitsEXT"/>.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessageTypeFlagsEXT(VkDebugUtilsMessageTypeFlagBitsEXT Value) : IVkFlags<VkDebugUtilsMessageTypeFlagBitsEXT>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDebugUtilsMessageTypeFlagBitsEXT value) =>
        this.Value.HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageTypeFlagsEXT(VkDebugUtilsMessageTypeFlagBitsEXT value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessageTypeFlagBitsEXT(VkDebugUtilsMessageTypeFlagsEXT value) => value.Value;
}
