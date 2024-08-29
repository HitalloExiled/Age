using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Interfaces;

public interface IPushConstant
{
    static abstract uint               Offset { get; }
    static abstract uint               Size   { get; }
    static abstract VkShaderStageFlags Stages { get; }
}
