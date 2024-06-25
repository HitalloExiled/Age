using Age.Rendering.Resources;
using Age.Rendering.Scene;

namespace Age.Rendering.RenderPasses;

public partial class SceneRenderGraphPass
{
    private readonly struct FrameResource
    {
        public Dictionary<int, UniformSet>     UniformSets { get; } = [];
        public Dictionary<Camera3D, UboHandle> CameraUbo   { get; } = [];

        public FrameResource() { }
    }
}

