using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Semaphores are a synchronization primitive that can be used to insert a dependency between queue operations or between a queue operation and the host. <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#glossary">Binary semaphores</see> have two states - signaled and unsignaled. <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#glossary">Timeline semaphores</see> have a strictly increasing 64-bit unsigned integer payload and are signaled with respect to a particular reference value. A semaphore can be signaled after execution of a queue operation is completed, and a queue operation can wait for a semaphore to become signaled before it begins execution. A timeline semaphore can additionally be signaled from the host with the <see cref="Vk.SignalSemaphore"/> command and waited on from the host with the <see cref="Vk.WaitSemaphores"/> command.
/// The internal data of a semaphore may include a reference to any resources and pending work associated with signal or unsignal operations performed on that semaphore object, collectively referred to as the semaphore’s payload. Mechanisms to import and export that internal data to and from semaphores are provided below. These mechanisms indirectly enable applications to share semaphore state between two or more semaphores and other synchronization primitives across process and API boundaries.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[DebuggerDisplay("{Value}")]
public record struct VkSemaphore(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkSemaphore(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkSemaphore value) => value.Value;
}
