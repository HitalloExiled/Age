namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineTessellationStateCreateInfo.html">VkPipelineTessellationStateCreateInfo</see>
/// </summary>
public unsafe struct VkPipelineTessellationStateCreateInfo
{
    public readonly VkStructureType SType;

    public void*                                  PNext;
    public VkPipelineTessellationStateCreateFlags Flags;
    public uint                                   PatchControlPoints;

    public VkPipelineTessellationStateCreateInfo() =>
        this.SType = VkStructureType.PipelineTessellationStateCreateInfo;
}
