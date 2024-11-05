using System.Runtime.InteropServices;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinput"></see>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RAWINPUT
    {
        [FieldOffset(0)]
        public RAWINPUTHEADER header;

        [FieldOffset(16)]
        public RAWMOUSE mouse;

        [FieldOffset(16)]
        public RAWKEYBOARD keyboard;

        [FieldOffset(16)]
        public RAWHID hid;
    }
}
