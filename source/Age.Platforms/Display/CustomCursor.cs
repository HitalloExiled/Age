#if LINUX
using Age.Core;
using Age.Numerics;
using Age.Platforms.Linux.LibWaylandClient;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    private struct CustomCursor
    {
		public wl_buffer*         Buffer;
		public NativeBuffer<uint> BufferData;

		public Point<int> Hotspot;
	}
}
#endif
