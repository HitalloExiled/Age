namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCreateFlagBits.html">VkBufferCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkBufferCreateFlags
{
    SparseBinding                    = 0x00000001,
    SparseResidency                  = 0x00000002,
    SparseAliased                    = 0x00000004,
    Protected                        = 0x00000008,
    DeviceAddressCaptureReplay       = 0x00000010,
    DescriptorBufferCaptureReplayEXT = 0x00000020,
    DeviceAddressCaptureReplayEXT    = DeviceAddressCaptureReplay,
    DeviceAddressCaptureReplayKHR    = DeviceAddressCaptureReplay,
}
