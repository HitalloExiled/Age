#if LINUX
using Age.Platforms.Linux.LibWaylandClient;

namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private struct BufferData
    {
        public required void*      Data;
        public required ulong      Size;
        public required wl_buffer* Buffer;
    }
}
#endif
