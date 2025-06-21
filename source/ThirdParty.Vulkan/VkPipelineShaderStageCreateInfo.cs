using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineShaderStageCreateInfo.html">VkPipelineShaderStageCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineShaderStageCreateInfo
{
    public readonly VkStructureType SType;

    public void*                            PNext;
    public VkPipelineShaderStageCreateFlags Flags;
    public VkShaderStageFlags               Stage;
    public VkHandle<VkShaderModule>         Module;
    public byte*                            PName;
    public VkSpecializationInfo*            PSpecializationInfo;

    public VkPipelineShaderStageCreateInfo() =>
        this.SType = VkStructureType.PipelineShaderStageCreateInfo;
}
