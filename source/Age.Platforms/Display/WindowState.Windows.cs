#if WINDOWS
using Age.Core;
using Age.Core.Collections;
using Age.Numerics;
using Age.Platforms.Windows;

namespace Age.Platforms.Display;

internal unsafe struct WindowState
{
    #region 8-bytes
    public HWND Handle;

    private NativeList<WindowMessage> messages;
    #endregion

    #region 4-bytes
    private UnsafeLock @lock;

    public Size<int> Size;
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
