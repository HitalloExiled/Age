namespace Age.Platforms.Windows;

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
