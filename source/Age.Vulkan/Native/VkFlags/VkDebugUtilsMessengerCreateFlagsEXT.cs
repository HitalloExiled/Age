using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native;

[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessengerCreateFlagsEXT(uint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool HasFlag(VkDebugUtilsMessengerCreateFlagsEXT value) =>
        (this.Value & value) == value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessengerCreateFlagsEXT(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(VkDebugUtilsMessengerCreateFlagsEXT value) => value.Value;
}
