namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/dataxchg/standard-clipboard-formats"></see>
    /// </summary>
    public enum STANDARD_CLIPBOARD_FORMATS : uint
    {
        CF_BITMAP = 2,
        CF_DIB = 8,
        CF_DIBV5 = 17,
        CF_DIF = 5,
        CF_DSPBITMAP = 0x0082,
        CF_DSPENHMETAFILE = 0x008E,
        CF_DSPMETAFILEPICT = 0x0083,
        CF_DSPTEXT = 0x0081,
        CF_ENHMETAFILE = 14,
        CF_GDIOBJFIRST = 0x0300,
        CF_GDIOBJLAST = 0x03FF,
        CF_HDROP = 15,
        CF_LOCALE = 16,
        CF_METAFILEPICT = 3,
        CF_OEMTEXT = 7,
        CF_OWNERDISPLAY = 0x0080,
        CF_PALETTE = 9,
        CF_PENDATA = 10,
        CF_PRIVATEFIRST = 0x0200,
        CF_PRIVATELAST = 0x02FF,
        CF_RIFF = 11,
        CF_SYLK = 4,
        CF_TEXT = 1,
        CF_TIFF = 6,
        CF_UNICODETEXT = 13,
        CF_WAVE = 12
    }
}
