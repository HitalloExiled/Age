namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-monitorinfo"></see>
    /// </summary>
    [Flags]
    public enum MONITOR_INFO_FLAGS
    {
        None                 = 0x0,
        MONITORINFOF_PRIMARY = 0x1,
    }
}
