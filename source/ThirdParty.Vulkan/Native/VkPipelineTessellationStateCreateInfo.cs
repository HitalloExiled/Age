namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineTessellationStateCreateInfo.html">VkPipelineTessellationStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineTessellationStateCreateInfo
{
    public readonly VkStructureType sType;

    public void*                                  pNext;
    public VkPipelineTessellationStateCreateFlags flags;
    public uint                                   patchControlPoints;

    public VkPipelineTessellationStateCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_STATE_CREATE_INFO;
}
