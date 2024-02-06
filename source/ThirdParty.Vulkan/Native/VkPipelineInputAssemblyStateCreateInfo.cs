using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineInputAssemblyStateCreateInfo.html">VkPipelineInputAssemblyStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineInputAssemblyStateCreateInfo
{
    public readonly VkStructureType SType;

    public void*                                   PNext;
    public VkPipelineInputAssemblyStateCreateFlags Flags;
    public VkPrimitiveTopology                     Topology;
    public VkBool32                                PrimitiveRestartEnable;

    public VkPipelineInputAssemblyStateCreateInfo() =>
        this.SType = VkStructureType.PipelineInputAssemblyStateCreateInfo;
}
