using Age.Rendering.Resources;

namespace Age.RenderPasses;

public partial class SceneRenderGraphPass
{
    private readonly struct FrameResource
    {
        public Dictionary<int, UniformSet> UniformSets { get; } = [];
        public Dictionary<int, UboHandle>  Ubo         { get; } = [];

        public FrameResource() { }
    }
}
