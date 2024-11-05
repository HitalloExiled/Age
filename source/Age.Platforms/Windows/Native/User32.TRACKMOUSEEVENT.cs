using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-trackmouseevent"></see>
    /// </summary>
    public struct TRACKMOUSEEVENT
    {
        public DWORD                 cbSize;
        public TRACKMOUSEEVENT_FLAGS dwFlags;
        public HWND                  hwndTrack;
        public DWORD                 dwHoverTime;
    }
}
