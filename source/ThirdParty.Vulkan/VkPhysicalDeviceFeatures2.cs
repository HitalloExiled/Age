using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public unsafe struct VkPhysicalDeviceFeatures2
{
    public VkStructureType          SType = VkStructureType.PhysicalDeviceFeatures2;
    public void*                    PNext;
    public VkPhysicalDeviceFeatures Features;

    public VkPhysicalDeviceFeatures2()
    { }
}
