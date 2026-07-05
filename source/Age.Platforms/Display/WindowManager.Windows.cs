#if WINDOWS
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Core.Exceptions;
using Age.Numerics;
using Age.Platforms.Windows;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    private static readonly Dictionary<Cursor, User32.IDC_STANDARD_CURSORS> standardCursors = new()
    {
        [Cursor.Arrow]              = User32.IDC_STANDARD_CURSORS.IDC_ARROW,
        [Cursor.Busy]               = User32.IDC_STANDARD_CURSORS.IDC_WAIT,
        [Cursor.Cross]              = User32.IDC_STANDARD_CURSORS.IDC_CROSS,
        [Cursor.DiagonalResizeNESW] = User32.IDC_STANDARD_CURSORS.IDC_SIZENWSE,
        [Cursor.DiagonalResizeNWSE] = User32.IDC_STANDARD_CURSORS.IDC_SIZENESW,
        [Cursor.Drag]               = User32.IDC_STANDARD_CURSORS.IDC_SIZEALL,
        [Cursor.Drop]               = User32.IDC_STANDARD_CURSORS.IDC_HAND,
        [Cursor.Forbidden]          = User32.IDC_STANDARD_CURSORS.IDC_NO,
        [Cursor.Hand]               = User32.IDC_STANDARD_CURSORS.IDC_HAND,
        [Cursor.Help]               = User32.IDC_STANDARD_CURSORS.IDC_HELP,
        [Cursor.HorizontalResize]   = User32.IDC_STANDARD_CURSORS.IDC_SIZEWE,
        [Cursor.HorizontalSplit]    = User32.IDC_STANDARD_CURSORS.IDC_SIZEWE,
        [Cursor.Move]               = User32.IDC_STANDARD_CURSORS.IDC_SIZEALL,
        [Cursor.Text]               = User32.IDC_STANDARD_CURSORS.IDC_IBEAM,
        [Cursor.VerticalResize]     = User32.IDC_STANDARD_CURSORS.IDC_SIZENS,
        [Cursor.VerticalSplit]      = User32.IDC_STANDARD_CURSORS.IDC_SIZENS,
        [Cursor.Wait]               = User32.IDC_STANDARD_CURSORS.IDC_WAIT,
    };

    private static Point<int>  previousMousePosition;
    private static long        previousMouseTime;
    private static MouseButton pressedButtons;

    public partial Cursor Cursor
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            this.UpdateCursor();
        }
    }

    public partial int CursorScale { get; set => field = value; }

    public partial bool CursorVisible
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            this.UpdateCursor();
        }
    }

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
    public static short LoWord(uint value) =>
        (short)((int)value & 0xffff);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short LoWord(nint value) =>
        LoWord((uint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short HiWord(uint value) =>
        (short)((value >> 16) & 0xffff);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static short HiWord(nint value) =>
        HiWord((uint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static ushort GetXLParam(LPARAM lParam) =>
        (ushort)short.Max(0, LoWord(lParam));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static ushort GetYLParam(LPARAM lParam) =>
        (ushort)short.Max(0, HiWord(lParam));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetKeyStateWParam(WPARAM wParam) =>
        LoWord(wParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static Modifier GetModifierState()
    {
        var modifiers = Modifier.None;

        if (User32.GetKeyState(User32.VIRTUAL_KEYS.VK_SHIFT) < 0)
        {
            modifiers |= Modifier.Shift;
        }

        if (User32.GetKeyState(User32.VIRTUAL_KEYS.VK_CONTROL) < 0)
        {
            modifiers |= Modifier.Ctrl;
        }

        if (User32.GetKeyState(User32.VIRTUAL_KEYS.VK_MENU) < 0)
        {
            modifiers |= Modifier.Alt;
        }

        if ((User32.GetKeyState(User32.VIRTUAL_KEYS.VK_LWIN) < 0) || (User32.GetKeyState(User32.VIRTUAL_KEYS.VK_RWIN) < 0))
        {
            modifiers |= Modifier.Meta;
        }

        return modifiers;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static WindowMouseEvent GetMouseEventArgs(MouseButton button, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam) =>
        new()
        {
            X              = GetXLParam(lParam),
            Y              = GetYLParam(lParam),
            Button         = button,
            LeftHanded     = GetPrimaryButtonLeftHanded(),
            Modifiers      = GetModifierState(),
            ScrollDelta    = msg is User32.WINDOW_MESSAGE.WM_MOUSEWHEEL or User32.WINDOW_MESSAGE.WM_MOUSEHWHEEL ? (GetWheelDeltaWParam(wParam) / (float)User32.WHEEL_DELTA) : 0,
            PressedButtons = pressedButtons,
            Relative       = default,
            Velocity       = default,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool GetPrimaryButtonLeftHanded() =>
        User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_SWAPBUTTON) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static int GetWheelDeltaWParam(WPARAM wParam) =>
        HiWord(wParam);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static WindowKeyEvent GetWindowKeyEvent(WPARAM wParam, LPARAM lParam, bool isPressed)
    {
        var vk       = (uint)wParam.Value;
        var key      = KeyMapping.GetKeycode(vk);
        var scancode = (uint)((lParam.Value >> 16) & 0xFF);
        var extended = ((lParam.Value >> 24) & 1) != 0;
        var location = KeyMapping.GetLocation(vk, scancode, extended);

        return new WindowKeyEvent
        {
            Key         = key,
            PhysicalKey = KeyMapping.GetScancode(scancode, extended),
            Modifiers   = GetModifierState(),
            Location    = location,
            IsPressed   = isPressed,
            Char        = default,
            Echo        = isPressed && ((lParam.Value >> 30) & 1) != 0,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Size<uint> GetWindowSize(HWND hwnd)
    {
        User32.GetWindowRect(hwnd, out var rect);

        return new((uint)(rect.right - rect.left), (uint)(rect.bottom - rect.top));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static User32.IDC_STANDARD_CURSORS ToIdcStandardCursors(Cursor cursor) =>
        standardCursors.TryGetValue(cursor, out var standardCursor) ? standardCursor: User32.IDC_STANDARD_CURSORS.IDC_ARROW;

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
        WindowMouseEvent mouseEvent = default;

        switch (msg)
        {
            case User32.WINDOW_MESSAGE.WM_SETFOCUS:
                state->AddMessage(WindowMessage.FocusIn());

                return 0;
            case User32.WINDOW_MESSAGE.WM_KILLFOCUS:
                state->AddMessage(WindowMessage.FocusOut());

                return 0;
            case User32.WINDOW_MESSAGE.WM_CHAR:
                state->AddMessage(WindowMessage.Input((char)wParam.Value));

                return 0;
            case User32.WINDOW_MESSAGE.WM_KEYDOWN:
            case User32.WINDOW_MESSAGE.WM_SYSKEYDOWN:
                state->AddMessage(WindowMessage.KeyDown(GetWindowKeyEvent(wParam, lParam, true)));

                return 0;
            case User32.WINDOW_MESSAGE.WM_KEYUP:
            case User32.WINDOW_MESSAGE.WM_SYSKEYUP:
                state->AddMessage(WindowMessage.KeyUp(GetWindowKeyEvent(wParam, lParam, false)));

                return 0;
            case User32.WINDOW_MESSAGE.WM_MOUSEMOVE:
                var pos      = new Point<int>(GetXLParam(lParam), GetYLParam(lParam));
                var delta    = pos - previousMousePosition;
                var relative = delta.Cast<short>();
                var elapsed  = Environment.TickCount64 - previousMouseTime;
                var velocity = elapsed > 0 ? (delta.Cast<float>() / elapsed).Cast<short>() : default;

                previousMousePosition = pos;
                previousMouseTime     = Environment.TickCount64;

                mouseEvent = GetMouseEventArgs(MouseButton.None, msg, wParam, lParam) with
                {
                    Relative = relative,
                    Velocity = velocity,
                };

                state->AddMessage(WindowMessage.MouseMove(mouseEvent));

                return 0;
            case User32.WINDOW_MESSAGE.WM_MOUSEWHEEL:
            case User32.WINDOW_MESSAGE.WM_MOUSEHWHEEL:
                var wheelDelta = GetWheelDeltaWParam(wParam);

                var wheellButton = msg switch
                {
                    User32.WINDOW_MESSAGE.WM_MOUSEWHEEL  => wheelDelta > 0 ? MouseButton.WheelUp : MouseButton.WheelDown,
                    User32.WINDOW_MESSAGE.WM_MOUSEHWHEEL => wheelDelta > 0 ? MouseButton.WheelRight : MouseButton.WheelLeft,
                    _ => default
                };

                mouseEvent = GetMouseEventArgs(wheellButton, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseWheel(mouseEvent));

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

                pressedButtons |= button;

                mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseDown(mouseEvent));

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

                pressedButtons |= button;

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

                pressedButtons &= ~button;

                User32.ReleaseCapture();

                mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseUp(mouseEvent));

                if (mouseEvent.IsPrimaryButtonPressed)
                {
                    state->AddMessage(WindowMessage.Click(mouseEvent));
                }

                return 0;
            case User32.WINDOW_MESSAGE.WM_XBUTTONDOWN:
                User32.SetCapture(hwnd);

                button = HiWord(wParam) == (short)User32.MOUSE_BUTTON.XBUTTON1 ? MouseButton.MbXbutton1 : MouseButton.MbXbutton2;

                pressedButtons |= button;

                mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseDown(mouseEvent));

                return 0;
            case User32.WINDOW_MESSAGE.WM_XBUTTONUP:
                button = HiWord(wParam) == (short)User32.MOUSE_BUTTON.XBUTTON1 ? MouseButton.MbXbutton1 : MouseButton.MbXbutton2;

                pressedButtons &= ~button;

                User32.ReleaseCapture();

                mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseUp(mouseEvent));

                if (mouseEvent.IsPrimaryButtonPressed)
                {
                    state->AddMessage(WindowMessage.Click(mouseEvent));
                }

                return 0;
            case User32.WINDOW_MESSAGE.WM_XBUTTONDBLCLK:
                User32.SetCapture(hwnd);

                button = HiWord(wParam) == (short)User32.MOUSE_BUTTON.XBUTTON1 ? MouseButton.MbXbutton1 : MouseButton.MbXbutton2;

                pressedButtons |= button;

                mouseEvent = GetMouseEventArgs(button, msg, wParam, lParam);

                state->AddMessage(WindowMessage.MouseDown(mouseEvent));
                state->AddMessage(WindowMessage.DoubleClick(mouseEvent));

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

    internal partial NativeArray<WindowMessage> FlushWindowEvents(Window window)
    {
        window.State->ClearMessages();

        while (User32.PeekMessageW(out var msg, window.Handle, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE))
        {
            User32.TranslateMessage(msg);
            User32.DispatchMessageW(msg);
        }

        return window.State->GetMessages();
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

    internal partial void UpdateCursor() =>
        User32.SetCursor(User32.LoadCursorW(default, ToIdcStandardCursors(this.Cursor)));
}
#endif
