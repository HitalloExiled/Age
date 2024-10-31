#if !Windows
#define Windows
#endif

#if Windows
using System.Runtime.CompilerServices;
using Age.Numerics;
using Age.Platforms.Windows.Native;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Display;

public partial class Window
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short LoWord(uint value) => (short)((int)value & 0xffff);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short LoWord(nint value) => LoWord((uint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short HiWord(uint value) => (short)((value >> 16) & 0xffff);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short HiWord(nint value) => HiWord((uint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static ushort GetXLParam(LPARAM lParam) => (ushort)LoWord(lParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static ushort GetYLParam(LPARAM lParam) => (ushort)HiWord(lParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetKeyStateWParam(WPARAM wParam) => LoWord(wParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetWheelDeltaWParam(WPARAM wParam) => HiWord(wParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static MouseEvent GetMouseEventArgs(MouseButton button, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam) =>
        new()
        {
            X             = GetXLParam(lParam),
            Y             = GetYLParam(lParam),
            Button        = button,
            PrimaryButton = GetPrimaryButton(),
            KeyStates     = (MouseKeyStates)GetKeyStateWParam(wParam),
            Delta         = msg == User32.WINDOW_MESSAGE.WM_MOUSEWHEEL ? (GetWheelDeltaWParam(wParam) / (float)User32.WHEEL_DELTA) : 0,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static MouseButton GetPrimaryButton() =>
        User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_SWAPBUTTON) == 0 ? MouseButton.Left : MouseButton.Right;

    private static User32.IDC_STANDARD_CURSORS GetPlatformCursor(CursorKind cursor) =>
        cursor switch
        {
            CursorKind.Arrow => User32.IDC_STANDARD_CURSORS.IDC_ARROW,
            CursorKind.Bean  => User32.IDC_STANDARD_CURSORS.IDC_IBEAM,
            CursorKind.Hand  => User32.IDC_STANDARD_CURSORS.IDC_HAND,
            _ => User32.IDC_STANDARD_CURSORS.IDC_ARROW,
        };

    private static LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
    {
        if (WindowsMap.TryGetValue(hwnd, out var window))
        {
            switch (msg)
            {
                case User32.WINDOW_MESSAGE.WM_SETCURSOR:
                    // User32.LoadCursorW();
                    User32.SetCursor(User32.LoadCursorW(default, GetPlatformCursor(window.Cursor)));

                    break;
                case User32.WINDOW_MESSAGE.WM_KEYDOWN:
                    window.KeyDown?.Invoke((Key)wParam.Value);
                    window.KeyPress?.Invoke((Key)wParam.Value);

                    break;
                case User32.WINDOW_MESSAGE.WM_KEYUP:
                    window.KeyUp?.Invoke((Key)wParam.Value);
                    window.KeyPress?.Invoke((Key)wParam.Value);

                    break;
                case User32.WINDOW_MESSAGE.WM_MOUSEMOVE:
                    window.MouseMove?.Invoke(GetMouseEventArgs(MouseButton.None, msg, wParam, lParam));

                    break;
                case User32.WINDOW_MESSAGE.WM_MOUSEWHEEL:
                    window.MouseWhell?.Invoke(GetMouseEventArgs(MouseButton.None, msg, wParam, lParam));

                    break;

                case User32.WINDOW_MESSAGE.WM_LBUTTONDOWN:
                case User32.WINDOW_MESSAGE.WM_MBUTTONDOWN:
                case User32.WINDOW_MESSAGE.WM_RBUTTONDOWN:
                    if (window.MouseDown != null)
                    {
                        User32.SetCapture(hwnd);

                        var button = msg switch
                        {
                            User32.WINDOW_MESSAGE.WM_LBUTTONDOWN => MouseButton.Left,
                            User32.WINDOW_MESSAGE.WM_MBUTTONDOWN => MouseButton.Middle,
                            User32.WINDOW_MESSAGE.WM_RBUTTONDOWN => MouseButton.Right,
                            _ => default,
                        };

                        window.MouseDown.Invoke(GetMouseEventArgs(button, msg, wParam, lParam));
                    }

                    break;
                case User32.WINDOW_MESSAGE.WM_LBUTTONDBLCLK:
                case User32.WINDOW_MESSAGE.WM_MBUTTONDBLCLK:
                case User32.WINDOW_MESSAGE.WM_RBUTTONDBLCLK:
                    if (window.MouseDown != null || window.DoubleClick != null)
                    {
                        var button = msg switch
                        {
                            User32.WINDOW_MESSAGE.WM_LBUTTONDBLCLK => MouseButton.Left,
                            User32.WINDOW_MESSAGE.WM_MBUTTONDBLCLK => MouseButton.Middle,
                            User32.WINDOW_MESSAGE.WM_RBUTTONDBLCLK => MouseButton.Right,
                            _ => default,
                        };

                        User32.SetCapture(hwnd);

                        var mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                        window.MouseDown?.Invoke(mouseEvent);
                        window.DoubleClick?.Invoke(mouseEvent);
                    }

                    break;

                case User32.WINDOW_MESSAGE.WM_LBUTTONUP:
                case User32.WINDOW_MESSAGE.WM_MBUTTONUP:
                case User32.WINDOW_MESSAGE.WM_RBUTTONUP:
                    if (window.MouseUp != null || window.Click != null)
                    {
                        var button = msg switch
                        {
                            User32.WINDOW_MESSAGE.WM_LBUTTONUP => MouseButton.Left,
                            User32.WINDOW_MESSAGE.WM_MBUTTONUP => MouseButton.Middle,
                            User32.WINDOW_MESSAGE.WM_RBUTTONUP => MouseButton.Right,
                            _ => default,
                        };

                        User32.ReleaseCapture();

                        var mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                        window.MouseUp?.Invoke(mouseEvent);

                        if (button == mouseEvent.PrimaryButton)
                        {
                            window.Click?.Invoke(mouseEvent);
                        }
                    }

                    break;
                case User32.WINDOW_MESSAGE.WM_CONTEXTMENU:
                    if (window.Context != null)
                    {
                        var x = GetXLParam(lParam);
                        var y = GetYLParam(lParam);

                        var point = new POINT { x = x, y = y };

                        User32.ScreenToClient(hwnd, point);

                        var contextEvent = new ContextEvent
                        {
                            X       = (ushort)point.x,
                            Y       = (ushort)point.y,
                            ScreenX = x,
                            ScreenY = y,
                        };

                        window.Context.Invoke(contextEvent);
                    }

                    break;
                case User32.WINDOW_MESSAGE.WM_SIZE:
                    if (window.Resized != null)
                    {
                        User32.GetWindowPlacement(hwnd, out var placement);

                        window.IsMaximized = placement.showCmd == User32.SHOW_WINDOW_COMMANDS.SW_SHOWMAXIMIZED;
                        window.IsMinimized = placement.showCmd == User32.SHOW_WINDOW_COMMANDS.SW_SHOWMINIMIZED;

                        var size = PlatformGetWindowSize(hwnd);

                        if (size.Width != window.Size.Width || size.Height != window.Size.Height)
                        {
                            window.size = size;

                            window.Resized.Invoke();
                        }
                    }

                    break;
                case User32.WINDOW_MESSAGE.WM_MOVING:
                    {
                        User32.GetWindowPlacement(hwnd, out var placement);

                        window.position = new(placement.rcNormalPosition.left, placement.rcNormalPosition.top);
                    }

                    break;
                case User32.WINDOW_MESSAGE.WM_CLOSE:
                    window.Close();

                    break;
                default:
                    break;
            }
        }

        return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    protected static unsafe void PlatformRegister(string className)
    {
        fixed (char* lpszClassName = className)
        {
            var windowClass = new User32.WNDCLASSEXW
            {
                cbSize        = (uint)sizeof(User32.WNDCLASSEXW),
                hbrBackground = default,
                // hCursor       = User32.LoadCursorW(default, User32.IDC_STANDARD_CURSORS.IDC_ARROW),
                // hCursor       = User32.LoadCursorW(default, User32.IDC_STANDARD_CURSORS.IDC_HAND),
                hIcon         = default,
                hIconSm       = default,
                hInstance     = default,
                lpszClassName = lpszClassName,
                lpszMenuName  = null,
                style         = User32.CLASS_STYLES.CS_DBLCLKS,
                lpfnWndProc   = new(WndProc),
            };

            if (User32.RegisterClassExW(windowClass) == 0)
            {
                throw new Exception("Failed to register window class");
            }
        }
    }

    protected static Size<uint> PlatformGetClientSize(HWND hwnd)
    {
        User32.GetClientRect(hwnd, out var rect);

        return new((uint)rect.right, (uint)rect.bottom);
    }

    protected static Size<uint> PlatformGetWindowSize(HWND hwnd)
    {
        User32.GetWindowRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
    }

    protected virtual void PlatformClose()
    {
        foreach (var child in this.Children)
        {
            child.IsClosed = true;

            WindowsMap.Remove(child.Handle);

            child.Closed?.Invoke();
        }

        User32.DestroyWindow(this.Handle);

        WindowsMap.Remove(this.Handle);
    }

    protected virtual void PlatformCreate(string title, Size<uint> size, Point<int> position, Window? parent)
    {
        if (!Registered)
        {
            Register("Age.Platforms.Window");
        }

        this.Handle = User32.CreateWindowExW(
            User32.WINDOW_STYLES_EX.WS_EX_APPWINDOW | User32.WINDOW_STYLES_EX.WS_EX_WINDOWEDGE,
            className,
            title,
            User32.WINDOW_STYLES.WS_VISIBLE | User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW,
            position.X,
            position.Y,
            (int)size.Width,
            (int)size.Height,
            parent?.Handle ?? default,
            default,
            default,
            0
        );

        if (this.Handle == default)
        {
            throw new Exception("Failed to create window on Windows OS.");
		}

        WindowsMap[this.Handle] = this;
    }

    protected void PlatformDoEvents()
    {
        while (User32.PeekMessageW(out var msg, this.Handle, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE) && !this.IsClosed)
        {
            User32.TranslateMessage(msg);
            User32.DispatchMessageW(msg);
        }
    }

    protected Size<uint> PlatformGetClientSize() =>
        PlatformGetClientSize(this.Handle);

    protected void PlatformHide() =>
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_HIDE);

    protected void PlatformMaximize() =>
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MAXIMIZE);

    protected void PlatformMinimize() =>
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MINIMIZE);

    protected void PlatformRestore() =>
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_RESTORE);

    protected void PlatformSetPosition(Point<int> value) => this.position = value;
    protected void PlatformSetSize(Size<uint> value) => this.size = value;
    protected void PlatformSetTitle(string value) => this.title = value;

    protected void PlatformShow() =>
        User32.ShowWindow(this.Handle, User32.SHOW_WINDOW_COMMANDS.SW_SHOW);
}
#endif
