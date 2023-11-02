using System.Diagnostics;
using System.Runtime.CompilerServices;
using Age.Vulkan.Interfaces;

namespace Age.Vulkan.Native.Flags.EXT;

/// <summary>
/// Reserved for future use.
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessengerCreateFlagsEXT(uint Value) : IVkFlags
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(uint value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessengerCreateFlagsEXT(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(VkDebugUtilsMessengerCreateFlagsEXT value) => value.Value;
}
