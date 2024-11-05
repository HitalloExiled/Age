using System.Runtime.InteropServices;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawmouse"></see>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RAWMOUSE
    {
        [FieldOffset(0)]
        public MOUSE_STATE usFlags;

        [FieldOffset(2)]
        public MOUSE_BUTTON_STATE ulButtons;

        [FieldOffset(2)]
        public DUMMYSTRUCTNAME DUMMYUNIONNAME;

        [FieldOffset(6)]
        public UINT ulRawButtons;

        [FieldOffset(8)]
        public LONG lLastX;

        [FieldOffset(16)]
        public LONG lLastY;

        [FieldOffset(32)]
        public ULONG ulExtraInformation;

        public struct DUMMYSTRUCTNAME
        {
            public USHORT usButtonFlags;
            public USHORT usButtonData;
        }
    }
}
