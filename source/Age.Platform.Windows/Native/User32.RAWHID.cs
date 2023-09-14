using System.Runtime.InteropServices;

namespace Age.Platform.Windows.Native;

internal static partial class User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWHID
    {
        public DWORD dwSizeHid;
        public DWORD dwCount;
        public BYTE  bRawData;
    }
}
