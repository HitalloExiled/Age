namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkStencilOpState.html">VkStencilOpState</see>
/// </summary>
public struct VkStencilOpState
{
    public VkStencilOp failOp;
    public VkStencilOp passOp;
    public VkStencilOp depthFailOp;
    public VkCompareOp compareOp;
    public uint        compareMask;
    public uint        writeMask;
    public uint        reference;
}
