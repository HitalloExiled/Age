namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferCreateFlagBits.html">VkBufferCreateFlagBits</see>
/// </summary>
[Flags]
public enum BufferCreateFlags
{
    SparseBinding                    = 0x00000001,
    SparseResidency                  = 0x00000002,
    SparseAliased                    = 0x00000004,
    Protected                        = 0x00000008,
    DeviceAddressCaptureReplay       = 0x00000010,
    DescriptorBufferCaptureReplayExt = 0x00000020,
    DeviceAddressCaptureReplayExt    = DeviceAddressCaptureReplay,
    DeviceAddressCaptureReplayKhr    = DeviceAddressCaptureReplay,
}
