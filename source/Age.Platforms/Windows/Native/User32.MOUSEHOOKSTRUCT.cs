using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mousehookstruct"></see>
    /// </summary>
    public struct MOUSEHOOKSTRUCT
    {
        public POINT     pt;
        public HWND      hwnd;
        public UINT      wHitTestCode;
        public ULONG_PTR dwExtraInfo;
    }
}
