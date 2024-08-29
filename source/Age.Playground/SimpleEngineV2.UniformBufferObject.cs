using Age.Numerics;

namespace Age.Playground;

public partial class SimpleEngineV2
{
    public struct UniformBufferObject
    {
        public Matrix4x4<float> Model;
        public Matrix4x4<float> View;
        public Matrix4x4<float> Proj;
    };
}
