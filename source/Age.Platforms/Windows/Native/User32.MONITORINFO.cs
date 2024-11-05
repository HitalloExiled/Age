using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-monitorinfo"></see>
    /// </summary>
    public struct MONITORINFO
    {
        public DWORD              cbSize;
        public RECT               rcMonitor;
        public RECT               rcWork;
        public MONITOR_INFO_FLAGS dwFlags;
    }
}
