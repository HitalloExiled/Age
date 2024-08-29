using System.Runtime.InteropServices;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER
    {
        public RAW_INPUT_TYPE dwType;
        public DWORD          dwSize;
        public HANDLE         hDevice;
        public WPARAM         wParam;
    }
}
