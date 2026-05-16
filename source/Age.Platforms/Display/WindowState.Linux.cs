#if LINUX
using Age.Platforms.Linux.Wayland;
using Age.Platforms.Linux.LibDecor;
using Age.Core;
using Age.Core.Collections;
using Age.Numerics;

namespace Age.Platforms.Display;

internal unsafe struct WindowState
{
    public required wl_surface* Surface;

    public wp_fractional_scale_v1* FractionalScale;
    public libdecor_frame*         Frame;
    public wl_callback*            FrameCallBack;
    public UnsafeLock              Lock;
    public libdecor_configuration* PendingLibdecorConfiguration;
    public wp_viewport*            Viewport;

    public required Size<int>  Size;
    public required Point<int> Position;

    public NativeList<WindowMessage> Messages;

    public WindowMode Mode;
    public bool       Suspended;

    public WindowState() =>
        this.Messages = [];

    public void Dispose() =>
        this.Messages.Dispose();
}
#endif
