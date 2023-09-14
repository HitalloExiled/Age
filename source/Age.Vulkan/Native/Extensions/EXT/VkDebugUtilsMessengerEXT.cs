using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native.Extensions.EXT;

[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessengerEXT(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessengerEXT(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkDebugUtilsMessengerEXT value) => value.Value;
}
