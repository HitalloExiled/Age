#if LINUX
using Age.Numerics;

namespace Age.Platforms.Display;

public unsafe partial class Window
{
    private struct ScreenData
    {
        public byte*      Make;
        public byte*      Model;
        public Size<int>  PhysicalSize;
        public Point<int> Position;
        public float      RefreshRate = -1;
        public int        Scale = 1;
        public Size<int>  Size;

        public ScreenData()
        { }
    };
}
#endif
