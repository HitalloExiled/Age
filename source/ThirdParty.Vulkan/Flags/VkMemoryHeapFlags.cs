namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkMemoryHeapFlagBits.html">VkMemoryHeapFlagBits</see>
/// </summary>
[Flags]
public enum VkMemoryHeapFlags
{
    DeviceLocal      = 0x00000001,
    MultiInstance    = 0x00000002,
    MultiInstanceBit = MultiInstance,
}
