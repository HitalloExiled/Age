#if LINUX
using Age.Core;

namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private record struct ExtendedState(SeatKind Kind, Pointer Data);
}
#endif
