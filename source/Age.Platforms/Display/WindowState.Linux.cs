#if LINUX
using Age.Platforms.Linux.LibWaylandClient;
using Age.Platforms.Linux.LibDecor;
using Age.Core;
using Age.Core.Collections;
using Age.Numerics;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Platforms.Display;

internal unsafe struct WindowState
{
    #region 8-bytes
    public wl_surface* Surface;

    public wp_fractional_scale_v1* FractionalScale;
    public libdecor_frame*         Frame;
    public wl_callback*            FrameCallBack;
    public libdecor_configuration* PendingLibdecorConfiguration;
    public wp_viewport*            Viewport;

    private NativeList<WindowMessage> messages;
    #endregion

    #region 4-bytes
    private UnsafeLock @lock;

    public Size<int> Size;
    #endregion

    #region 1-byte
    public WindowMode Mode;
    public bool       Suspended;
    #endregion

    private WindowState(wl_surface* surface, in Size<int> size)
    {
        this.Surface  = surface;
        this.Size     = size;
        this.messages = [];
    }

    public static WindowState* Allocate(wl_surface* surface, in Size<int> size) =>
        NativeMemory.Alloc(new WindowState(surface, size));

    public static void Free(WindowState* windowState)
    {
        windowState->Dispose();

        NativeMemory.Free(windowState);
    }

    public void Dispose()
    {
        viewporter.wp_viewport_destroy(this.Viewport);
        lib_wayland_client.wl_surface_destroy(this.Surface);

        this.messages.Dispose();
    }

    public void AddMessage(in WindowMessage windowMessage)
    {
        using (UnsafeLock.Lock(ref this.@lock))
        {
            this.messages.Add(windowMessage);
        }
    }

    public void ClearMessages()
    {
        using (UnsafeLock.Lock(ref this.@lock))
        {
            this.messages.Clear();
        }
    }

    public NativeArray<WindowMessage> GetMessages()
    {
        using (UnsafeLock.Lock(ref this.@lock))
        {
            return this.messages.ToNativeArray();
        }
    }
}
#endif
