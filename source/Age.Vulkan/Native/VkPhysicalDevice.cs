using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native;

/// <summary>
/// <para>Opaque handle to a physical device object.</para>
/// <para>Vulkan separates the concept of physical and logical devices. A physical device usually represents a single complete implementation of Vulkan (excluding instance-level functionality) available to the host, of which there are a finite number. A logical device represents an instance of that implementation with its own state and resources independent of other logical devices.</para>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkPhysicalDevice(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkPhysicalDevice(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkPhysicalDevice value) => value.Value;
}
