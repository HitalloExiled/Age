using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;

public partial class CanvasShader
{
    public struct PushConstant : IPushConstant
    {
        static uint               IPushConstant.Offset { get; }
        static unsafe uint        IPushConstant.Size   { get; } = (uint)sizeof(PushConstant);
        static VkShaderStageFlags IPushConstant.Stages { get; } = VkShaderStageFlags.Vertex | VkShaderStageFlags.Fragment;

        public Size<float>  ViewportSize;
        public Rect<float>  Rect;
        public Color        Color;
    }
}