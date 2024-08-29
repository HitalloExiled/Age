using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

#pragma warning disable CS0649

internal static partial class User32
{
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }
}
