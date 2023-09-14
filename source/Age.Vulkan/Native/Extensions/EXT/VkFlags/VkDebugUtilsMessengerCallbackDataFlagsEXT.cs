using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native.Extensions.EXT.VkFlags;

/// <summary>
/// Reserved for future use.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessengerCallbackDataFlagsEXT(uint Value) : IVkFlags
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(uint value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessengerCallbackDataFlagsEXT(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(VkDebugUtilsMessengerCallbackDataFlagsEXT value) => value.Value;
}
