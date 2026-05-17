namespace Age.Platforms.Windows;

internal static partial class User32
{
    public unsafe struct HOOKPROC
    {
        public delegate LRESULT Function(int code, WINDOW_MESSAGE wParam, LPARAM lParam);

        public required delegate* unmanaged<
            int            /* code */,
            WINDOW_MESSAGE /* wParam */,
            LPARAM         /* lParam */,
            LRESULT
        > Value;
    }
}
