#if LINUX
using Age.Platforms.Linux.Wayland;
using Age.Platforms.Linux.LibDecor;
using Age.Core;
using Age.Core.Collections;
using Age.Numerics;

namespace Age.Platforms.Display;

internal unsafe struct WindowState
{
    #region 8-bytes
    public required wl_surface* Surface;

    public wp_fractional_scale_v1* FractionalScale;
    public libdecor_frame*         Frame;
    public wl_callback*            FrameCallBack;
    public libdecor_configuration* PendingLibdecorConfiguration;
    public wp_viewport*            Viewport;

    private NativeList<WindowMessage> messages;
    #endregion

    #region 4-bytes
    private UnsafeLock @lock;

    public required Size<int> Size;
    #endregion

    #region 1-byte
    public WindowMode Mode;
    public bool       Suspended;
    #endregion

    public WindowState() =>
        this.messages = [];

    public void Dispose() =>
        this.messages.Dispose();

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
