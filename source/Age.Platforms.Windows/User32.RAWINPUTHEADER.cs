using System.Runtime.InteropServices;

namespace Age.Platforms.Windows;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputheader"></see>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER
    {
        public RAW_INPUT_TYPE dwType;
        public DWORD          dwSize;
        public HANDLE         hDevice;
        public WPARAM         wParam;
    }
}
