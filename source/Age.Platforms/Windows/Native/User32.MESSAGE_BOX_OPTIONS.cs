namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messageboxw"></see>
    /// </summary>
    [Flags]
    public enum MESSAGE_BOX_OPTIONS : uint
    {
        MB_OK                   = 0x000000,
        MB_OKCANCEL             = 0x000001,
        MB_ABORTRETRYIGNORE     = 0x000002,
        MB_YESNOCANCEL          = 0x000003,
        MB_YESNO                = 0x000004,
        MB_RETRYCANCEL          = 0x000005,
        MB_CANCELTRYCONTINUE    = 0x000006,
        MB_ICONSTOP             = 0x000010,
        MB_ICONERROR            = MB_ICONSTOP,
        MB_ICONHAND             = MB_ICONSTOP,
        MB_ICONQUESTION         = 0x000020,
        MB_ICONWARNING          = 0x000030,
        MB_ICONEXCLAMATION      = MB_ICONWARNING,
        MB_ICONASTERISK         = 0x000040,
        MB_ICONINFORMATION      = MB_ICONASTERISK,
        MB_USERICON             = 0x000080,
        MB_DEFBUTTON1           = MB_OK,
        MB_DEFBUTTON2           = 0x000100,
        MB_DEFBUTTON3           = 0x000200,
        MB_DEFBUTTON4           = 0x000300,
        MB_APPLMODAL            = MB_OK,
        MB_SYSTEMMODAL          = 0x001000,
        MB_TASKMODAL            = 0x002000,
        MB_HELP                 = 0x004000,
        MB_NOFOCUS              = 0x008000,
        MB_SETFOREGROUND        = 0x00010000,
        MB_DEFAULT_DESKTOP_ONLY = 0x00020000,
        MB_TOPMOST              = 0x00040000,
        MB_RIGHT                = 0x00080000,
        MB_RTLREADING           = 0x00100000,
        MB_SERVICE_NOTIFICATION = 0x00200000,
    }
}
