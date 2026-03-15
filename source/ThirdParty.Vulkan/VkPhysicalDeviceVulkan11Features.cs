using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public unsafe struct VkPhysicalDeviceVulkan11Features
{
    public VkStructureType SType = VkStructureType.PhysicalDeviceVulkan11Features;
    public void*           PNext;
    public VkBool32        StorageBuffer16BitAccess;
    public VkBool32        UniformAndStorageBuffer16BitAccess;
    public VkBool32        StoragePushConstant16;
    public VkBool32        StorageInputOutput16;
    public VkBool32        Multiview;
    public VkBool32        MultiviewGeometryShader;
    public VkBool32        MultiviewTessellationShader;
    public VkBool32        VariablePointersStorageBuffer;
    public VkBool32        VariablePointers;
    public VkBool32        ProtectedMemory;
    public VkBool32        SamplerYcbcrConversion;
    public VkBool32        ShaderDrawParameters;

    public VkPhysicalDeviceVulkan11Features()
    { }
}
