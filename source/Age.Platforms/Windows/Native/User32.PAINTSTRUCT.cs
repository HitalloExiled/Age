using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-paintstruct"></see>
    /// </summary>
    public unsafe struct PAINTSTRUCT
    {
        public HDC        hdc;
        public BOOL       fErase;
        public RECT       rcPaint;
        public BOOL       fRestore;
        public BOOL       fIncUpdate;
        public fixed byte rgbReserved[32];
    }
}
