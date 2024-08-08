using Age.Numerics;

namespace Age.Internal;

internal struct UniformBufferObject
{
    public Matrix4x4<float> Model;
    public Matrix4x4<float> View;
    public Matrix4x4<float> Proj;
};
