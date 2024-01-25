namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkQueueFlagBits.html">VkQueueFlagBits</see>
/// </summary>
[Flags]
public enum QueueFlags
{
    Graphics       = 0x00000001,
    Compute        = 0x00000002,
    Transfer       = 0x00000004,
    SparseBinding  = 0x00000008,
    Protected      = 0x00000010,
    VideoDecodeKhr = 0x00000020,

#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeKhr = 0x00000040,
#endif

    OpticalFlowNv  = 0x00000100,
}
