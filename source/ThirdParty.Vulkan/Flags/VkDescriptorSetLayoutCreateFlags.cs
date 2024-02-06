namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutCreateFlagBits.html">VkDescriptorSetLayoutCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkDescriptorSetLayoutCreateFlags
{
    UpdateAfterBindPool          = 0x00000002,
    PushDescriptorKHR            = 0x00000001,
    DescriptorBufferEXT          = 0x00000010,
    EmbeddedImmutableSamplersEXT = 0x00000020,
    IndirectBindableNV           = 0x00000080,
    HostOnlyPoolEXT              = 0x00000004,
    UpdateAfterBindPoolEXT       = UpdateAfterBindPool,
    HostOnlyPoolValve            = HostOnlyPoolEXT,
}
