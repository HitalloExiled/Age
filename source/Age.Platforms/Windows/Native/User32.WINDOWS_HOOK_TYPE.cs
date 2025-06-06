﻿namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexw"></see>
    /// </summary>
    public enum WINDOWS_HOOK_TYPE
    {
        WH_MSGFILTER       = -1,
        WH_JOURNALRECORD   = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD        = 2,
        WH_GETMESSAGE      = 3,
        WH_CALLWNDPROC     = 4,
        WH_CBT             = 5,
        WH_SYSMSGFILTER    = 6,
        WH_MOUSE           = 7,
        WH_HARDWARE        = 8,
        WH_DEBUG           = 9,
        WH_SHELL           = 10,
        WH_FOREGROUNDIDLE  = 11,
        WH_CALLWNDPROCRET  = 12,
        WH_KEYBOARD_LL     = 13,
        WH_MOUSE_LL        = 14,
    }
}
