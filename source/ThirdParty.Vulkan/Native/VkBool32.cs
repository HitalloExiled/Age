using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThirdParty.Vulkan.Native;

[DebuggerDisplay("{Value == 1}")]
public record struct VkBool32(uint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkBool32(uint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(VkBool32 value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkBool32(bool value) => new(value ? 1u : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator bool(VkBool32 value) => value.Value == 1;
}
