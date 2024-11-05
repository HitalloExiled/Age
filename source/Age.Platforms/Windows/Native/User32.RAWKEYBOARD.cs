using System.Runtime.InteropServices;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawkeyboard"></see>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWKEYBOARD
    {
        public USHORT MakeCode;
        public USHORT Flags;
        public USHORT Reserved;
        public USHORT VKey;
        public UINT   Message;
        public ULONG  ExtraInformation;
    }
}
