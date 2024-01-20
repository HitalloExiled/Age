using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Types;

/// <summary>
/// <para>Opaque handle to a queue object.</para>
/// <para>Creating a logical device also creates the queues associated with that device. The queues to create are described by a set of <see cref="VkDeviceQueueCreateInfo"/> structures that are passed to <see cref="Vk.CreateDevice"/> in pQueueCreateInfos.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkQueue(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkQueue(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkQueue value) => value.Value;
}
