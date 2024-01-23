#if !Windows
#define Windows
#endif

#if Windows
using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using Age.Platforms.Windows.Native;
using Age.Platforms.Windows.Native.Types;

namespace Age.Rendering.Display;

public partial class Window
{
    private nint handler;

    private static unsafe void PlatformRegister(string className)
    {
        fixed (char* lpszClassName = className)
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

            if (User32.RegisterClassExW(windowClass) == 0)
            {
                throw new Exception("Failed to register window class");
            }
        }
    }

    private static Size<uint> PlatformGetClientSize(HWND hwnd)
    {
        User32.GetClientRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
    }

    private static Size<uint> PlatformGetWindowSize(HWND hwnd)
    {
        User32.GetWindowRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
    }

    private static LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
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

                        var size = PlatformGetWindowSize(hwnd);

                        if (size.Width != window.Size.Width || size.Height != window.Size.Height)
                        {
                            window.size = size;

                            window.SizeChanged?.Invoke();
                        }
                    }

                    return 0;
                case User32.WINDOW_MESSAGE.WM_MOVING:
                    {
                        User32.GetWindowPlacement(hwnd, out var placement);

                        window.position = new(placement.rcNormalPosition.left, placement.rcNormalPosition.top);
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

    private void PlatformClose()
    {
        foreach (var child in this.children)
        {
            child.Closed = true;

            renderer!.Context.DestroySurfaceContext(child.Context);

            windows.Remove(child.handler);

            child.WindowClosed?.Invoke();
        }

        User32.DestroyWindow(this.handler);

        renderer!.Context.DestroySurfaceContext(this.Context);

        windows.Remove(this.handler);
    }

    [MemberNotNull(nameof(Context))]
    private void PlatformCreate(string title, Size<uint> size, Point<int> position, Window? parent)
    {
        this.handler = User32.CreateWindowExW(
            User32.WINDOW_STYLES_EX.WS_EX_APPWINDOW | User32.WINDOW_STYLES_EX.WS_EX_WINDOWEDGE,
            className,
            title,
            User32.WINDOW_STYLES.WS_VISIBLE | User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW,
            position.X,
            position.Y,
            (int)size.Width,
            (int)size.Height,
            parent?.handler ?? default,
            default,
            default,
            0
        );

        if (this.handler == default)
        {
            throw new Exception("Failed to create window on Windows OS.");
		}

        var clientSize = PlatformGetClientSize(this.handler);

        this.Context = renderer!.Context.CreateSurfaceContext(this.handler, clientSize);

        this.SizeChanged += () =>
        {
            this.Context.Size   = this.ClientSize;
            this.Context.Hidden = this.Minimized || !this.Visible;
        };

        windows[this.handler] = this;
    }

    private void PlatformDoEvents()
    {
        while (User32.PeekMessageW(out var msg, this.handler, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE) && !this.Closed)
        {
            User32.TranslateMessage(msg);
            User32.DispatchMessageW(msg);
        }
    }

    private Size<uint> PlatformGetClientSize() =>
        PlatformGetClientSize(this.handler);

    private void PlatformHide() =>
        User32.ShowWindow(this.handler, User32.SHOW_WINDOW_COMMANDS.SW_HIDE);

    private void PlatformMaximize() =>
        User32.ShowWindow(this.handler, User32.SHOW_WINDOW_COMMANDS.SW_MAXIMIZE);

    private void PlatformMinimize() =>
        User32.ShowWindow(this.handler, User32.SHOW_WINDOW_COMMANDS.SW_MINIMIZE);

    private void PlatformRestore() =>
        User32.ShowWindow(this.handler, User32.SHOW_WINDOW_COMMANDS.SW_RESTORE);

    private void PlatformSetPosition(Point<int> value) => this.position = value;
    private void PlatformSetSize(Size<uint> value) => this.size = value;
    private void PlatformSetTitle(string value) => this.title = value;

    private void PlatformShow() =>
        User32.ShowWindow(this.handler, User32.SHOW_WINDOW_COMMANDS.SW_SHOW);
}
#endif
