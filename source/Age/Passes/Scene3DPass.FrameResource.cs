using Age.Rendering.Resources;
using Age.Resources;
using Age.Scenes;

namespace Age.Passes;

public partial class Scene3DPass
{
    protected readonly struct FrameResource
    {
        public Dictionary<(Camera3D, Material), UniformSet>   UniformSets { get; } = [];
        public Dictionary<(Camera3D, Mesh), BufferHandlePair> Ubo         { get; } = [];

        public FrameResource() { }
    }
}
