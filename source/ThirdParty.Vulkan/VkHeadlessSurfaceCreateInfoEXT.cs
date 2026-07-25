using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public unsafe struct VkHeadlessSurfaceCreateInfoEXT
{
    public VkStructureType                 SType = VkStructureType.HeadlessSurfaceCreateInfoEXT;
    public void*                           PNext;
    public VkHeadlessSurfaceCreateFlagsEXT Flags;

    public VkHeadlessSurfaceCreateInfoEXT()
    { }
}
