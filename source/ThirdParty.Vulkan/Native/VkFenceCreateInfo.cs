using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkFenceCreateInfo.html">VkFenceCreateInfo</see>
/// </summary>
public unsafe struct VkFenceCreateInfo
{
    public readonly VkStructureType SType;

    public void*              PNext;
    public VkFenceCreateFlags Flags;

    public VkFenceCreateInfo() =>
        this.SType = VkStructureType.FenceCreateInfo;
}
