#if WINDOWS
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;
using Age.Core.Exceptions;
using Age.Numerics;
using Age.Platforms.Windows;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    public partial WindowManager(string id)
    {
        SingletonViolationException.ThrowIfNoSingleton(Instance);

        Instance = this;

        this.Id = id;

        fixed (char* lpszClassName = id)
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
                style         = User32.CLASS_STYLES.CS_DBLCLKS,
                lpfnWndProc   = new() { Value = &WndProc },
            };

            if (User32.RegisterClassExW(windowClass) == 0)
            {
                throw new Exception("Failed to register window class");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short LoWord(uint value) => (short)((int)value & 0xffff);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short LoWord(nint value) => LoWord((uint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short HiWord(uint value) => (short)((value >> 16) & 0xffff);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short HiWord(nint value) => HiWord((uint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static ushort GetXLParam(LPARAM lParam) => (ushort)short.Max(0, LoWord(lParam));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static ushort GetYLParam(LPARAM lParam) => (ushort)short.Max(0, HiWord(lParam));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetKeyStateWParam(WPARAM wParam) => LoWord(wParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetWheelDeltaWParam(WPARAM wParam) => HiWord(wParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static WindowMouseEvent GetMouseEventArgs(MouseButton button, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam) =>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Size<uint> GetWindowSize(HWND hwnd)
    {
        User32.GetWindowRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static User32.IDC_STANDARD_CURSORS ToIdcStandardCursors(Cursor cursor) =>
        cursor switch
        {
            Cursor.Arrow            => User32.IDC_STANDARD_CURSORS.IDC_ARROW,
            Cursor.Busy             => User32.IDC_STANDARD_CURSORS.IDC_WAIT,
            Cursor.Cross            => User32.IDC_STANDARD_CURSORS.IDC_CROSS,
            Cursor.DiagonalResize1  => User32.IDC_STANDARD_CURSORS.IDC_SIZENWSE,
            Cursor.DiagonalResize2  => User32.IDC_STANDARD_CURSORS.IDC_SIZENESW,
            Cursor.Hand             => User32.IDC_STANDARD_CURSORS.IDC_HAND,
            Cursor.Help             => User32.IDC_STANDARD_CURSORS.IDC_HELP,
            Cursor.HorizontalResize => User32.IDC_STANDARD_CURSORS.IDC_SIZEWE,
            Cursor.Move             => User32.IDC_STANDARD_CURSORS.IDC_SIZEALL,
            Cursor.Progress         => User32.IDC_STANDARD_CURSORS.IDC_APPSTARTING,
            Cursor.Text             => User32.IDC_STANDARD_CURSORS.IDC_IBEAM,
            Cursor.Unavailable      => User32.IDC_STANDARD_CURSORS.IDC_NO,
            Cursor.VerticalResize   => User32.IDC_STANDARD_CURSORS.IDC_SIZENS,
            _ => User32.IDC_STANDARD_CURSORS.IDC_ARROW,
        };

    [UnmanagedCallersOnly]
    private static LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == User32.WINDOW_MESSAGE.WM_NCCREATE)
        {
            var createStruct = (User32.CREATESTRUCTW*)(void*)lParam;

            User32.SetWindowLongPtrW(hwnd, User32.WINDOW_LONG_INDEX.GWLP_USERDATA, (LONG_PTR)(LONG*)(void*)createStruct->lpCreateParams);
        }

        var state = (WindowState*)(void*)User32.GetWindowLongPtrW(hwnd, User32.WINDOW_LONG_INDEX.GWLP_USERDATA);

        MouseButton      button     = default;
        Key              key        = default;
        WindowMouseEvent mouseEvent = default;

        switch (msg)
        {
            case User32.WINDOW_MESSAGE.WM_CHAR:
                state->AddMessage(WindowMessage.Input((char)wParam.Value));

                return 0;
            case User32.WINDOW_MESSAGE.WM_KEYDOWN:
                key = (Key)wParam.Value;

                state->AddMessage(WindowMessage.KeyDown(key));
                state->AddMessage(WindowMessage.KeyPress(key));

                return 0;
            case User32.WINDOW_MESSAGE.WM_KEYUP:
                key = (Key)wParam.Value;

                state->AddMessage(WindowMessage.KeyUp(key));
                state->AddMessage(WindowMessage.KeyPress(key));

                return 0;
            case User32.WINDOW_MESSAGE.WM_MOUSEMOVE:
                state->AddMessage(WindowMessage.MouseMove(GetMouseEventArgs(MouseButton.None, msg, wParam, lParam)));

                return 0;
            case User32.WINDOW_MESSAGE.WM_MOUSEWHEEL:
                state->AddMessage(WindowMessage.MouseWheel(GetMouseEventArgs(MouseButton.None, msg, wParam, lParam)));

                return 0;
            case User32.WINDOW_MESSAGE.WM_LBUTTONDOWN:
            case User32.WINDOW_MESSAGE.WM_MBUTTONDOWN:
            case User32.WINDOW_MESSAGE.WM_RBUTTONDOWN:
                User32.SetCapture(hwnd);

                button = msg switch
                {
                    User32.WINDOW_MESSAGE.WM_LBUTTONDOWN => MouseButton.Left,
                    User32.WINDOW_MESSAGE.WM_MBUTTONDOWN => MouseButton.Middle,
                    User32.WINDOW_MESSAGE.WM_RBUTTONDOWN => MouseButton.Right,
                    _ => default,
                };

                state->AddMessage(WindowMessage.MouseDown(GetMouseEventArgs(button, msg, wParam, lParam)));

                return 0;
            case User32.WINDOW_MESSAGE.WM_LBUTTONDBLCLK:
            case User32.WINDOW_MESSAGE.WM_MBUTTONDBLCLK:
            case User32.WINDOW_MESSAGE.WM_RBUTTONDBLCLK:
                User32.SetCapture(hwnd);

                button = msg switch
                {
                    User32.WINDOW_MESSAGE.WM_LBUTTONDBLCLK => MouseButton.Left,
                    User32.WINDOW_MESSAGE.WM_MBUTTONDBLCLK => MouseButton.Middle,
                    User32.WINDOW_MESSAGE.WM_RBUTTONDBLCLK => MouseButton.Right,
                    _ => default,
                };

                mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseDown(mouseEvent));
                state->AddMessage(WindowMessage.DoubleClick(mouseEvent));

                return 0;
            case User32.WINDOW_MESSAGE.WM_LBUTTONUP:
            case User32.WINDOW_MESSAGE.WM_MBUTTONUP:
            case User32.WINDOW_MESSAGE.WM_RBUTTONUP:
                button = msg switch
                {
                    User32.WINDOW_MESSAGE.WM_LBUTTONUP => MouseButton.Left,
                    User32.WINDOW_MESSAGE.WM_MBUTTONUP => MouseButton.Middle,
                    User32.WINDOW_MESSAGE.WM_RBUTTONUP => MouseButton.Right,
                    _ => default,
                };

                User32.ReleaseCapture();

                mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseUp(mouseEvent));

                if (button == mouseEvent.PrimaryButton)
                {
                    state->AddMessage(WindowMessage.Click(mouseEvent));
                }

                return 0;
            case User32.WINDOW_MESSAGE.WM_CONTEXTMENU:
                var x = GetXLParam(lParam);
                var y = GetYLParam(lParam);

                var point = new POINT { x = x, y = y };

                User32.ScreenToClient(hwnd, point);

                var contextEvent = new WindowContextEvent
                {
                    X       = (ushort)point.x,
                    Y       = (ushort)point.y,
                    ScreenX = x,
                    ScreenY = y,
                };

                state->AddMessage(WindowMessage.Context(contextEvent));

                return 0;
            case User32.WINDOW_MESSAGE.WM_SIZE:
            case User32.WINDOW_MESSAGE.WM_SIZING:
                User32.GetClientRect(hwnd, out var rect);

                var size = new Size<int>(rect.right, rect.bottom);

                if (state->Size != size)
                {
                    state->Size = size;
                    state->AddMessage(WindowMessage.Resized());
                }

                return 0;

            case User32.WINDOW_MESSAGE.WM_CLOSE:
                state->AddMessage(WindowMessage.Closed());

                return 0;
            case User32.WINDOW_MESSAGE.WM_SETCURSOR:
                if ((User32.HIT_TEST)LoWord(lParam) == User32.HIT_TEST.HTCLIENT)
                {
                    state->AddMessage(WindowMessage.CursorChanged());

                    return 1;
                }

                break;
        }

        return User32.DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    protected override partial void OnDisposed(bool disposing)
    { }

    internal partial void CloseWindow(Window window) =>
        User32.DestroyWindow(window.Handle);

    internal partial WindowState* CreateWindow(string title, Size<int> size, Window? parent)
    {
        var state = NativeMemory.Alloc(new WindowState { Size = size });

        state->Handle = User32.CreateWindowExW(
            User32.WINDOW_STYLES_EX.WS_EX_APPWINDOW | User32.WINDOW_STYLES_EX.WS_EX_WINDOWEDGE,
            this.Id,
            title,
            User32.WINDOW_STYLES.WS_VISIBLE | User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW,
            default,
            default,
            size.Width,
            size.Height,
            parent?.Handle ?? default,
            default,
            default,
            (nint)state
        );

        return state;
    }

    internal partial void FlushWindowEvents(Window window)
    {
        while (User32.PeekMessageW(out var msg, window.Handle, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE))
        {
            User32.TranslateMessage(msg);
            User32.DispatchMessageW(msg);
        }
    }

    internal partial string? GetClipboardData(Window window)
    {
        if (User32.OpenClipboard(window.Handle))
        {
            var text = User32.GetClipboardTextData();

            User32.CloseClipboard();

            return text;
        }

        return null;
    }

    internal partial void HideWindow(Window window) =>
        User32.ShowWindow(window.Handle, User32.SHOW_WINDOW_COMMANDS.SW_HIDE);

    internal partial void MaximizeWindow(Window window) =>
        User32.ShowWindow(window.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MAXIMIZE);

    internal partial void MinimizeWindow(Window window) =>
        User32.ShowWindow(window.Handle, User32.SHOW_WINDOW_COMMANDS.SW_MINIMIZE);

    internal partial void RestoreWindow(Window window) =>
        User32.ShowWindow(window.Handle, User32.SHOW_WINDOW_COMMANDS.SW_RESTORE);

    internal partial void SetWindowClipboardData(Window window, string value)
    {
        if (User32.OpenClipboard(window.Handle))
        {
            User32.EmptyClipboard();
            User32.SetClipboardData(value);
            User32.CloseClipboard();
        }
    }

    internal partial void SetWindowTitle(Window window, string value) =>
        User32.SetWindowText(window.Handle, value);

    internal partial void ShowWindow(Window window) =>
        User32.ShowWindow(window.Handle, User32.SHOW_WINDOW_COMMANDS.SW_SHOW);

    internal void UpdateCursor(Cursor cursor) =>
        User32.SetCursor(User32.LoadCursorW(default, ToIdcStandardCursors(cursor)));
}
#endif
