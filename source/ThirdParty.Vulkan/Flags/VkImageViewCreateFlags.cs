namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageViewCreateFlagBits.html">VkImageViewCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkImageViewCreateFlags
{
    FragmentDensityMapDynamicEXT     = 0x00000001,
    DescriptorBufferCaptureReplayEXT = 0x00000004,
    FragmentDensityMapDeferredEXT    = 0x00000002,
}
