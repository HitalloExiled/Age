namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageViewCreateFlagBits.html">VkImageViewCreateFlagBits</see>
/// </summary>
[Flags]
public enum ImageViewCreateFlags
{
    FragmentDensityMapDynamicExt     = 0x00000001,
    DescriptorBufferCaptureReplayExt = 0x00000004,
    FragmentDensityMapDeferredExt    = 0x00000002,
}
