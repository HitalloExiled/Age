namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorPoolCreateFlagBits.html">VkDescriptorPoolCreateFlagBits</see>
/// </summary>
[Flags]
public enum VkDescriptorPoolCreateFlags
{
    FreeDescriptorSet          = 0x00000001,
    UpdateAfterBind            = 0x00000002,
    HostOnlyEXT                = 0x00000004,
    AllowOverallocationSetsNV  = 0x00000008,
    AllowOverallocationPoolsNV = 0x00000010,
    UpdateAfterBindEXT         = UpdateAfterBind,
    HostOnlyValve              = HostOnlyEXT,
}
