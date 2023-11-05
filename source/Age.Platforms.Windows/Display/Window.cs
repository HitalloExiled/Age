using Age.Numerics;
using Age.Platforms.Windows.Native;
using Age.Platforms.Windows.Native.Types;

using PlatformWindow = Age.Platforms.Display.Window;

namespace Age.Platforms.Windows.Display;

public class Window(HWND handle, string title, Size<uint> size, Point<int> position, PlatformWindow? parent = null) : PlatformWindow(title, size, position, parent), IDisposable
{
    internal event Action<Window>? Destroyed;

    private bool disposed;

    public HWND Handle { get; } = handle;

    public override Size<uint> ClientSize
    {
        get
        {
            User32.GetClientRect(this.Handle, out var rect);

            return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
        }
    }

    ~Window() =>
        this.Dispose(false);

    internal void SetMaximized(bool value) => this.Maximized = value;
    internal void SetMinimized(bool value) => this.Minimized = value;
    internal new void NotifySizeChanged() => base.NotifySizeChanged();

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            this.Close();
            this.disposed = true;
        }
    }

    public override void Close()
    {
        if (!this.Closed)
        {
            this.Closed = true;

            this.NotifyClosed();
            this.Destroyed?.Invoke(this);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public override void Hide()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_HIDE);

        this.Visible = false;
    }

    public override void Maximize()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MAXIMIZE);

        this.Maximized = true;
        this.Minimized = false;
    }

    public override void Minimize()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MINIMIZE);

        this.Maximized = false;
        this.Minimized = true;
    }

    public override void Restore()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_RESTORE);

        this.Maximized = false;
        this.Minimized = false;
    }

    public override void Show()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_SHOW);

        this.Visible = true;
    }
}
