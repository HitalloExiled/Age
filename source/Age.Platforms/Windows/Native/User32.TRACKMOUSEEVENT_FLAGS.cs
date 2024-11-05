namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-trackmouseevent"></see>
    /// </summary>
    [Flags]
    public enum TRACKMOUSEEVENT_FLAGS : uint
    {
        TME_HOVER     = 0x00000001,
        TME_LEAVE     = 0x00000002,
        TME_NONCLIENT = 0x00000010,
        TME_QUERY     = 0x40000000,
        TME_CANCEL    = 0x80000000,
    }
}
