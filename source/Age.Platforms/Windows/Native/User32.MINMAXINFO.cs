using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

#pragma warning disable CS0649

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-minmaxinfo"></see>
    /// </summary>
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }
}
