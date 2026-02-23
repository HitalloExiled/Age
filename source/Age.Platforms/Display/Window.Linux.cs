#if LINUX
using Age.Numerics;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Display;

public partial class Window
{
    protected static void PlatformRegister(string className) => throw new NotImplementedException();
    public Size<uint> PlatformGetClientSize(HWND hwnd) => throw new NotImplementedException();
    protected static Size<uint> PlatformGetWindowSize(HWND hwnd) => throw new NotImplementedException();
    protected static void PlatformSetCursor(Cursor value) => throw new NotImplementedException();
    protected virtual void PlatformClose() => throw new NotImplementedException();
    protected virtual void PlatformCreate(string title, Size<uint> size, Point<int> position, Window? parent) => throw new NotImplementedException();
    protected string? PlatformGetClipboardData() => throw new NotImplementedException();
    protected void PlatformSetClipboardData(string value) => throw new NotImplementedException();
    protected void PlatformDoEvents() => throw new NotImplementedException();
    protected Size<uint> PlatformGetClientSize() => throw new NotImplementedException();
    protected void PlatformHide() => throw new NotImplementedException();
    protected void PlatformMaximize() => throw new NotImplementedException();
    protected void PlatformMinimize() => throw new NotImplementedException();
    protected void PlatformRestore() => throw new NotImplementedException();
    protected void PlatformSetPosition(in Point<int> value) => throw new NotImplementedException();
    protected void PlatformSetSize(in Size<uint> value) => throw new NotImplementedException();
    protected void PlatformSetTitle(string value) => throw new NotImplementedException();
    protected void PlatformShow() => throw new NotImplementedException();
}
#endif
