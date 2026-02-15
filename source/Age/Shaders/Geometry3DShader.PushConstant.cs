using Age.Numerics;

namespace Age.Shaders;

public abstract partial class Geometry3DShader
{
    public struct PushConstant
    {
        // [16-bytes boundary]
        public Color Color;
    }
}
