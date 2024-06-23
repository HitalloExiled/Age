using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.RenderPasses;

public partial class SceneRenderGraphPass
{
    private record struct UboHandle(nint Handle, Buffer Buffer);
}

