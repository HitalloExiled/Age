using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native;

[DebuggerDisplay("{Value}")]
public record struct VkInstance(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkInstance(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkInstance value) => value.Value;
}
