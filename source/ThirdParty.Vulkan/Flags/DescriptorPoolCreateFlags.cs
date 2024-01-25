namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkDescriptorPoolCreateFlagBits.html">VkDescriptorPoolCreateFlagBits</see>
/// </summary>
[Flags]
public enum DescriptorPoolCreateFlags
{
    FreeDescriptorSet          = 0x00000001,
    UpdateAfterBind            = 0x00000002,
    HostOnlyExt                = 0x00000004,
    AllowOverallocationSetsNv  = 0x00000008,
    AllowOverallocationPoolsNv = 0x00000010,
    UpdateAfterBindExt         = UpdateAfterBind,
    HostOnlyValve              = HostOnlyExt,
}
