using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Resources;

namespace Age.Rendering.Interfaces;

public interface IWindow
{
    Size<uint> ClientSize { get; }
    bool       Closed     { get; }
    Content    Content    { get; }
    bool       Minimized  { get; }
    Surface    Surface    { get; }
    bool       Visible    { get; }

    Size<uint> Size { get; set; }
}