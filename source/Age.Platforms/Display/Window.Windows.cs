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
    private static short GetXLParam(LPARAM lParam) => LoWord(lParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static short GetYLParam(LPARAM lParam) => HiWord(lParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetKeyStateWParam(WPARAM wParam) => LoWord(wParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetWheelDeltaWParam(WPARAM wParam) => HiWord(wParam);

    private static LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
    {
        if (WindowsMap.TryGetValue(hwnd, out var window))
        {
            switch (msg)
            {
                case User32.WINDOW_MESSAGE.WM_KEYDOWN:
                    window.KeyDown?.Invoke((Key)wParam.Value);
                    window.KeyPress?.Invoke((Key)wParam.Value);

                    break;
                case User32.WINDOW_MESSAGE.WM_KEYUP:
                    window.KeyUp?.Invoke((Key)wParam.Value);
                    window.KeyPress?.Invoke((Key)wParam.Value);

                    break;
                case User32.WINDOW_MESSAGE.WM_MOUSEMOVE:
                    {
                        var x = GetXLParam(lParam);
                        var y = GetYLParam(lParam);

                        window.MouseMove?.Invoke(x, y);
                    }

                    break;
                case User32.WINDOW_MESSAGE.WM_MOUSEWHEEL:
                    {
                        var keys  = (MouseKeyStates)GetKeyStateWParam(wParam);
                        var whell = GetWheelDeltaWParam(wParam);
                        var delta = whell / (float)User32.WHEEL_DELTA;


                        window.MouseWhell?.Invoke(delta, keys);
                    }

                    break;
                case User32.WINDOW_MESSAGE.WM_LBUTTONDBLCLK:
                    window.DoubleClick?.Invoke(MouseButton.Left);

                    break;
                case User32.WINDOW_MESSAGE.WM_LBUTTONDOWN:
                    window.ClickDown?.Invoke(MouseButton.Left);
                    window.Click?.Invoke(MouseButton.Left);

                    break;
                case User32.WINDOW_MESSAGE.WM_LBUTTONUP:
                    window.ClickUp?.Invoke(MouseButton.Left);
                    window.Click?.Invoke(MouseButton.Left);

                    break;
                case User32.WINDOW_MESSAGE.WM_MBUTTONDBLCLK:
                    window.DoubleClick?.Invoke(MouseButton.Middle);

                    break;
                case User32.WINDOW_MESSAGE.WM_MBUTTONDOWN:
                    window.ClickDown?.Invoke(MouseButton.Middle);

                    break;
                case User32.WINDOW_MESSAGE.WM_MBUTTONUP:
                    window.ClickUp?.Invoke(MouseButton.Middle);

                    break;
                case User32.WINDOW_MESSAGE.WM_RBUTTONDBLCLK:
                    window.DoubleClick?.Invoke(MouseButton.Right);

                    break;
                case User32.WINDOW_MESSAGE.WM_RBUTTONDOWN:
                    window.ClickDown?.Invoke(MouseButton.Right);
                    window.Click?.Invoke(MouseButton.Right);

                    break;
                case User32.WINDOW_MESSAGE.WM_RBUTTONUP:
                    window.ClickUp?.Invoke(MouseButton.Right);
                    window.Click?.Invoke(MouseButton.Right);

                    break;
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

    protected static Size<uint> PlatformGetClientSize(HWND hwnd)
    {
        User32.GetClientRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
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
            child.Closed = true;

            WindowsMap.Remove(child.Handle);

            child.WindowClosed?.Invoke();
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
        while (User32.PeekMessageW(out var msg, this.Handle, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE) && !this.Closed)
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
