
using Age.Platform.Windows.Native;
using Age.Platform.Windows.Native.Types;

namespace Age.Platform.Windows.Display;

public class Window : IDisposable
{
    private bool disposed;

    internal event Action<Window>? Destroyed;

    public bool Closed    { get; private set; }
    public bool Maximized { get; private set; }
    public bool Minimized { get; private set; }
    public bool Visible   { get; private set; }

    public HWND    Handle { get; }
    public Window? Parent { get; }

    public int    Height { get; set; }
    public string Title  { get; set; }
    public int    Width  { get; set; }
    public int    X      { get; set; }
    public int    Y      { get; set; }

    internal unsafe Window(string title, int width, int height, int x, int y, Window? parent = null)
    {
        this.Title  = title;
        this.Width  = width;
        this.Height = height;
        this.X      = x;
        this.Y      = y;
        this.Parent = parent;

        using var className = new LPCWSTR(nameof(Window));

        var windowClass = new User32.WNDCLASSEXW
        {
            cbSize        = (uint)sizeof(User32.WNDCLASSEXW),
            hbrBackground = default,
            hCursor       = User32.LoadCursorW(default, User32.IDC_STANDARD_CURSORS.IDC_ARROW),
            hIcon         = default,
            hIconSm       = default,
            hInstance     = default,
            lpszClassName = className,
            lpszMenuName  = null,
            style         = 0,
            lpfnWndProc   = new(this.WndProc)
        };

        var classId = User32.RegisterClassExW(windowClass);

        this.Handle = User32.CreateWindowExW(
            User32.WINDOW_STYLES_EX.WS_EX_APPWINDOW | User32.WINDOW_STYLES_EX.WS_EX_WINDOWEDGE,
            className,
            title,
            User32.WINDOW_STYLES.WS_VISIBLE | User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW,
            x,
            y,
            width,
            height,
            parent?.Handle ?? default,
            default,
            default,
            0
        );

        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_SHOWDEFAULT);
    }

    ~Window() =>
        this.Dispose(false);

    private LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case User32.WINDOW_MESSAGE.WM_PAINT:
                _ = User32.BeginPaint(hwnd, out var ps);

                User32.EndPaint(hwnd, ps);
                return 0;
            case User32.WINDOW_MESSAGE.WM_CLOSE:
                this.Close();

                return 0;
            default:
                break;
        }

        return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

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

    public void Close()
    {
        if (!this.Closed)
        {
            this.Closed = true;

            this.Destroyed?.Invoke(this);

            User32.DestroyWindow(this.Handle);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void DoEvents()
    {
        while (User32.PeekMessageW(out var msg, this.Handle, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE) && !this.Closed)
        {
            User32.DispatchMessageW(msg);
        }
    }

    public void Hide()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_HIDE);

        this.Visible = false;
    }

    public void Maximize()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MAXIMIZE);

        this.Maximized = true;
        this.Minimized = false;
    }

    public void Minimize()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MINIMIZE);

        this.Maximized = false;
        this.Minimized = true;
    }

    public void Restore()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_RESTORE);

        this.Maximized = false;
        this.Minimized = false;
    }

    public void Show()
    {
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_SHOW);

        this.Visible = true;
    }
}
