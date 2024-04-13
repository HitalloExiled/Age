using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Resources;

namespace Age.Rendering.Interfaces;

public interface IWindow
{
    event Action SizeChanged;

    Size<uint> ClientSize { get; }
    bool       Closed     { get; }
    NodeTree   Tree       { get; }
    bool       Minimized  { get; }
    Surface    Surface    { get; }
    bool       Visible    { get; }

    Size<uint> Size     { get; set; }
    Point<int> Position { get; set; }
}
