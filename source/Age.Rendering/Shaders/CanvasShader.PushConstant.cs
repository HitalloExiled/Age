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
        public Point<float> UV0;
        public Point<float> UV1;
        public Point<float> UV2;
        public Point<float> UV3;
        public Color        Color;
    }
}
