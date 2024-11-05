namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="">https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-peekmessagew</see>
    /// </summary>
    public enum PEEK_MESSAGE : uint
    {
        PM_NOREMOVE       = 0x0000,
        PM_REMOVE         = 0x0001,
        PM_NOYIELD        = 0x0002,
        PM_QS_INPUT       = QUEUE_STATUS.QS_INPUT << 16,
        PM_QS_PAINT       = QUEUE_STATUS.QS_PAINT << 16,
        PM_QS_POSTMESSAGE = (QUEUE_STATUS.QS_POSTMESSAGE | QUEUE_STATUS.QS_HOTKEY | QUEUE_STATUS.QS_TIMER) << 16,
        PM_QS_SENDMESSAGE = QUEUE_STATUS.QS_SENDMESSAGE << 16
    }
}
