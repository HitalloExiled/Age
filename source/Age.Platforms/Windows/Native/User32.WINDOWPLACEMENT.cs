using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-windowplacement"></see>
    /// </summary>
    public struct WINDOWPLACEMENT
    {
        public UINT                 length;
        public UINT                 flags;
        public SHOW_WINDOW_COMMANDS showCmd;
        public POINT                ptMinPosition;
        public POINT                ptMaxPosition;
        public RECT                 rcNormalPosition;
        public RECT                 rcDevice;
    }
}
