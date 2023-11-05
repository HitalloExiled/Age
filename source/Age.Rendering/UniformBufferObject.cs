using Age.Numerics;

namespace Age.Rendering.Vulkan;

public struct UniformBufferObject
{
    public Matrix4x4<float> Model;
    public Matrix4x4<float> Projection;
    public Matrix4x4<float> View;
};
