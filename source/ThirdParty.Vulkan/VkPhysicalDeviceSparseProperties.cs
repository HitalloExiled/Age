namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceSparseProperties.html">VkPhysicalDeviceSparseProperties</see>
/// </summary>
public struct VkPhysicalDeviceSparseProperties
{
    public VkBool32 ResidencyStandard2DBlockShape;
    public VkBool32 ResidencyStandard2DMultisampleBlockShape;
    public VkBool32 ResidencyStandard3DBlockShape;
    public VkBool32 ResidencyAlignedMipSize;
    public VkBool32 ResidencyNonResidentStrict;
}
