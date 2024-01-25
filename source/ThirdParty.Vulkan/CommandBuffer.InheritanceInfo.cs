using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class CommandBuffer
{
    /// <inheritdoc cref="VkCommandBufferInheritanceInfo" />
    public unsafe record InheritanceInfo : NativeReference<VkCommandBufferInheritanceInfo>
    {
    }
}


