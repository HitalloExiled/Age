using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Vulkan;

namespace Age.Rendering.Interfaces;

public interface IWindow
{
    Size<uint>     ClientSize { get; }
    Content        Content    { get; }
    SurfaceContext Context    { get; }

    Size<uint> Size { get; set; }
}
