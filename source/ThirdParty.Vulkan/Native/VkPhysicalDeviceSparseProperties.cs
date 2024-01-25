namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceSparseProperties.html">VkPhysicalDeviceSparseProperties</see>
/// </summary>
public struct VkPhysicalDeviceSparseProperties
{
    public VkBool32 residencyStandard2DBlockShape;
    public VkBool32 residencyStandard2DMultisampleBlockShape;
    public VkBool32 residencyStandard3DBlockShape;
    public VkBool32 residencyAlignedMipSize;
    public VkBool32 residencyNonResidentStrict;
}
