namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/window-class-styles"></see>
    /// </summary>
    [Flags]
    public enum CLASS_STYLES : uint
    {
        CS_BYTEALIGNCLIENT = 0x1000,
        CS_BYTEALIGNWINDOW = 0x2000,
        CS_CLASSDC         = 0x0040,
        CS_DBLCLKS         = 0x0008,
        CS_DROPSHADOW      = 0x00020000,
        CS_GLOBALCLASS     = 0x4000,
        CS_HREDRAW         = 0x0002,
        CS_NOCLOSE         = 0x0200,
        CS_OWNDC           = 0x0020,
        CS_PARENTDC        = 0x0080,
        CS_SAVEBITS        = 0x0800,
        CS_VREDRAW         = 0x0001,
    }
}
