using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Scene;

namespace Age.Rendering.Interfaces;

public interface IWindow
{
    event Action<short, short> MouseMove;
    event Action               SizeChanged;

    Size<uint> ClientSize { get; }
    bool       Closed     { get; }
    SceneTree  Tree       { get; }
    bool       Minimized  { get; }
    Surface    Surface    { get; }
    bool       Visible    { get; }

    Size<uint> Size     { get; set; }
    Point<int> Position { get; set; }
}
