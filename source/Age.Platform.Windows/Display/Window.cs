using Age.Platform.Windows.Native;
using Age.Platform.Windows.Native.Types;

namespace Age.Platform.Windows.Display;

public class Window : IDisposable
{
    internal event Action<Window>? Destroyed;

    public event Action? WindowClosed;
    public event Action? SizeChanged;

    private static readonly Dictionary<HWND, Window> windows = new();

    private bool disposed;

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

        fixed (char* lpszClassName = nameof(Window))
        {
            var windowClass = new User32.WNDCLASSEXW
            {
                cbSize        = (uint)sizeof(User32.WNDCLASSEXW),
                hbrBackground = default,
                hCursor       = User32.LoadCursorW(default, User32.IDC_STANDARD_CURSORS.IDC_ARROW),
                hIcon         = default,
                hIconSm       = default,
                hInstance     = default,
                lpszClassName = lpszClassName,
                lpszMenuName  = null,
                style         = 0,
                lpfnWndProc   = new(WndProc),
            };

            var classId = User32.RegisterClassExW(windowClass);

            this.Handle = User32.CreateWindowExW(
                User32.WINDOW_STYLES_EX.WS_EX_APPWINDOW | User32.WINDOW_STYLES_EX.WS_EX_WINDOWEDGE,
                nameof(Window),
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

            windows[this.Handle] = this;
        }
    }

    ~Window() =>
        this.Dispose(false);

    private static unsafe LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
    {
        if (windows.TryGetValue(hwnd, out var window))
        {
            switch (msg)
            {
                case User32.WINDOW_MESSAGE.WM_SIZE:
                    {
                        User32.GetWindowPlacement(hwnd, out var placement);

                        window.Maximized = placement.showCmd == User32.SHOW_WINDOW_COMMANDS.SW_SHOWMAXIMIZED;
                        window.Minimized = placement.showCmd == User32.SHOW_WINDOW_COMMANDS.SW_SHOWMINIMIZED;

                        var width  = placement.rcNormalPosition.right - placement.rcNormalPosition.left;
                        var height = placement.rcNormalPosition.bottom - placement.rcNormalPosition.top;

                        if (width != window.Width || height != window.Height)
                        {
                            window.Height = height;
                            window.Width  = width;

                            window.NotifySizeChanged();
                        }
                    }

                    return 0;
                case User32.WINDOW_MESSAGE.WM_MOVING:
                    {
                        User32.GetWindowPlacement(hwnd, out var placement);

                        window.X = placement.rcNormalPosition.left;
                        window.Y = placement.rcNormalPosition.top;
                    }

                    return 0;
                case User32.WINDOW_MESSAGE.WM_CLOSE:
                    window.Close();

                    return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
                default:
                    break;
            }
        }

        return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    private void NotifySizeChanged() =>
        this.SizeChanged?.Invoke();


    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            windows.Remove(this.Handle);
            this.Close();
            this.disposed = true;
        }
    }

    public void Close()
    {
        if (!this.Closed)
        {
            this.Closed = true;

            this.WindowClosed?.Invoke();
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
