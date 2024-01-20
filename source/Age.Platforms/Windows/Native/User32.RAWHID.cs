using System.Runtime.InteropServices;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

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
