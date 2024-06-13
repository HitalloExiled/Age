using Age.Rendering.Interfaces;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;

public partial class GeometryShader
{
    public readonly struct PushConstant : IPushConstant
    {
        public static uint               Offset { get; }
        public static uint               Size   { get; }
        public static VkShaderStageFlags Stages { get; }
    }
}
