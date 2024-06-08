using Age.Numerics;

namespace Age.Rendering.RenderPasses;

public partial class GeometryRenderGraphPass
{
    public struct UniformBufferObject
    {
        public Matrix4x4<float> Model;
        public Matrix4x4<float> View;
        public Matrix4x4<float> Proj;
    };
}
