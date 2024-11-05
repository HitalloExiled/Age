namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-createpolygonrgn"></see>
    /// </summary>
    public enum FILL_MODE
    {
        ALTERNATE,
        WINDING,
    }
}
