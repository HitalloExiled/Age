#if LINUX
namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private enum SeatKind : byte
    {
        Cursor,
        Keyboard,
    }
}
#endif
