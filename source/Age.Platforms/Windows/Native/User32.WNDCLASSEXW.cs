using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-wndclassexw"></see>
    /// </summary>
    public struct WNDCLASSEXW
    {
        public UINT         cbSize;
        public CLASS_STYLES style;
        public WNDPROC      lpfnWndProc;
        public int          cbClsExtra;
        public int          cbWndExtra;
        public HINSTANCE    hInstance;
        public HICON        hIcon;
        public HCURSOR      hCursor;
        public HBRUSH       hbrBackground;
        public LPCWSTR      lpszMenuName;
        public LPCWSTR      lpszClassName;
        public HICON        hIconSm;
    }
}
