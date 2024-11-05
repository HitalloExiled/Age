namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdata"></see>
    /// </summary>
    public enum RAW_INPUT_DATA_COMMAND_FLAGS : uint
    {
        RID_HEADER = 268435461U,
        RID_INPUT  = 268435459U,
    }
}
