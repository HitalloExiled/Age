using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan.Flags;

namespace Age.Shaders;

public partial class CanvasShader
{
    public struct PushConstant : IPushConstant
    {
        static uint               IPushConstant.Offset { get; }
        static unsafe uint        IPushConstant.Size   { get; } = (uint)sizeof(PushConstant);
        static VkShaderStageFlags IPushConstant.Stages { get; } = VkShaderStageFlags.Vertex | VkShaderStageFlags.Fragment;

        // [16-bytes boundary]
        public Color Color;

        // [8-bytes boundary]
        public Size<float>      Viewport;
        public Matrix3x2<float> Transform;
        public Rect<float>      Rect;
        public UVRect           UV;
        public Border           Border;

        // [4-bytes boundary]
        public Flags Flags;
    }
}
