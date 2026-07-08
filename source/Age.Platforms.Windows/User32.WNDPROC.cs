namespace Age.Platforms.Windows;

internal static partial class User32
{
    public unsafe struct WNDPROC
    {
        public required delegate* unmanaged<
            HWND           /* hwnd */,
            WINDOW_MESSAGE /* msg */,
            WPARAM         /* wParam */,
            LPARAM         /* lParam */,
            LRESULT
        > Value;
    }
}
