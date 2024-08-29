using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.RenderPasses;

public partial class SceneRenderGraphPass
{
    private record struct UboHandle(nint Handle, Buffer Buffer);
}

