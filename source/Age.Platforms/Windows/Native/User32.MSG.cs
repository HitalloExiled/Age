using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    public struct MSG
    {
        public HWND   hwnd;
        public UINT   message;
        public WPARAM wParam;
        public LPARAM lParam;
        public DWORD  time;
        public POINT  pt;
    }
}
