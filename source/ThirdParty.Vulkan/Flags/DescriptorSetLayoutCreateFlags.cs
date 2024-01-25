namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorSetLayoutCreateFlagBits.html">VkDescriptorSetLayoutCreateFlagBits</see>
/// </summary>
[Flags]
public enum DescriptorSetLayoutCreateFlags
{
    UpdateAfterBindPool          = 0x00000002,
    PushDescriptorKhr            = 0x00000001,
    DescriptorBufferExt          = 0x00000010,
    EmbeddedImmutableSamplersExt = 0x00000020,
    IndirectBindableNv           = 0x00000080,
    HostOnlyPoolExt              = 0x00000004,
    UpdateAfterBindPoolExt       = UpdateAfterBindPool,
    HostOnlyPoolValve            = HostOnlyPoolExt,
}
