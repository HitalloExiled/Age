#if LINUX
using Age.Platforms.Linux.Wayland;
using Age.Platforms.Linux.LibDecor;
using Age.Core.Collections;
using Age.Numerics;

namespace Age.Platforms.Display;

public unsafe partial class Window
{
    private struct WindowState
    {
        public required wl_surface* Surface;

        public wp_fractional_scale_v1* FractionalScale;
        public wl_callback*            FrameCallBack;
        public libdecor_frame*         Frame;
        public libdecor_configuration* PendingLibdecorConfiguration;
        public wp_viewport*            Viewport;

        public required Size<int>  Size;
        public required Point<int> Position;

        public NativeList<Message> Messages = [];

        public WindowMode Mode;
        public bool       Suspended;

        public WindowState()
        { }

        public void Dispose() =>
            this.Messages.Dispose();
    }
}
#endif
