namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-rawinputheader"></see>
    /// </summary>
    public enum RAW_INPUT_TYPE
    {
        RIM_TYPEMOUSE    = 0,
        RIM_TYPEKEYBOARD = 1,
        RIM_TYPEHID      = 2
    }
}
