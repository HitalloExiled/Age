using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-createstructw"></see>
    /// </summary>
    public struct CREATESTRUCTW
    {
        public LPVOID    lpCreateParams;
        public HINSTANCE hInstance;
        public HMENU     hMenu;
        public HWND      hwndParent;
        public int       cy;
        public int       cx;
        public int       y;
        public int       x;
        public LONG      style;
        public LPCWSTR   lpszName;
        public LPCWSTR   lpszClass;
        public DWORD     dwExStyle;
    }
}
