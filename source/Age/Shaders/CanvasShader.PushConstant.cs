using Age.Numerics;

namespace Age.Shaders;

public partial class CanvasShader
{
    public struct PushConstant
    {
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
