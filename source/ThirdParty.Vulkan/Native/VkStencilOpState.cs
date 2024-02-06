using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkStencilOpState.html">VkStencilOpState</see>
/// </summary>
public struct VkStencilOpState
{
    public VkStencilOp FailOp;
    public VkStencilOp PassOp;
    public VkStencilOp DepthFailOp;
    public VkCompareOp CompareOp;
    public uint        CompareMask;
    public uint        WriteMask;
    public uint        Reference;
}
