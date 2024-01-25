namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineInputAssemblyStateCreateInfo.html">VkPipelineInputAssemblyStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineInputAssemblyStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                   pNext;
    public VkPipelineInputAssemblyStateCreateFlags flags;
    public VkPrimitiveTopology                     topology;
    public VkBool32                                primitiveRestartEnable;

    public VkPipelineInputAssemblyStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO;
}
