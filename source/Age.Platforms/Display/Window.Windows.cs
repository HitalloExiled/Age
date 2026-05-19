#if WINDOWS
using Age.Numerics;

namespace Age.Platforms.Display;

public unsafe partial class Window
{
    public nint Handle => this.State->Handle;
}
#endif
