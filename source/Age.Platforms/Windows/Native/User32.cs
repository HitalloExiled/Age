using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Platforms.Windows.Native.Types;

[assembly: InternalsVisibleTo("Age.Rendering")]

namespace Age.Platforms.Windows.Native;

internal static unsafe partial class User32
{
    public const uint   HOVER_DEFAULT      = 0xFFFFFFFF;
    public const uint   USER_TIMER_MINIMUM = 0x0000000A;
    public const uint   USER_TIMER_MAXIMUM = 0x7FFFFFFF;
    public const ushort WHEEL_DELTA        = 120;

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-activate"></see>
    /// </summary>
    public const int WA_INACTIVE = 0;

    /// <inheritdoc cref="WA_INACTIVE" />
    public const int WA_ACTIVE = 1;

    /// <inheritdoc cref="WA_INACTIVE" />
    public const int WA_CLICKACTIVE = 2;

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-adjustwindowrectex"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL AdjustWindowRectEx(LPRECT lpRect, WINDOW_STYLES dwStyle, BOOL bMenu, WINDOW_STYLES_EX dwExStyle);

    /// <inheritdoc cref="AdjustWindowRectEx" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL AdjustWindowRectEx(ref RECT lpRect, WINDOW_STYLES dwStyle, BOOL bMenu, WINDOW_STYLES_EX dwExStyle);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-beginpaint"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HDC BeginPaint(HWND hWnd, LPPAINTSTRUCT lpPaint);

    /// <inheritdoc cref="BeginPaint" />
    [LibraryImport(nameof(User32))]
    public static partial HDC BeginPaint(HWND hWnd, out PAINTSTRUCT lpPaint);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-callnexthookex"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial LRESULT CallNextHookEx(HHOOK hhk, int nCode, WINDOW_MESSAGE wParam, LPARAM lParam);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-callwindowprocw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial LRESULT CallWindowProcW(WNDPROC lpPrevWndFunc, HWND hWnd, WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-clienttoscreen"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL ClientToScreen(HWND hWnd, LPPOINT lpPoint);

    /// <inheritdoc cref="ClientToScreen(HWND, LPPOINT)" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL ClientToScreen(HWND hWnd, ref POINT lpPoint);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-clipcursor"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL ClipCursor(RECT* lpRect);

    /// <inheritdoc cref="ClipCursor" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL ClipCursor(in RECT lpRect);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-createpolygonrgn"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HRGN CreatePolygonRgn(POINT* pptl, int cPoint, FILL_MODE iMode);

    /// <inheritdoc cref="CreatePolygonRgn" />
    public static HRGN CreatePolygonRgn(POINT[] points, FILL_MODE iMode)
    {
        fixed (POINT* pptl = points)
        {
            return CreatePolygonRgn(pptl, points.Length, iMode);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createwindowexw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HWND CreateWindowExW(
        WINDOW_STYLES_EX dwExStyle,
        LPCWSTR          lpClassName,
        LPCWSTR          lpWindowName,
        WINDOW_STYLES    dwStyle,
        int              x,
        int              y,
        int              nWidth,
        int              nHeight,
        HWND             hWndParent,
        HMENU            hMenu,
        HINSTANCE        hInstance,
        LPVOID           lpParam
    );

    /// <inheritdoc cref="CreateWindowExW" />
    public static HWND CreateWindowExW<T>(
        WINDOW_STYLES_EX dwExStyle,
        string?          className,
        string?          windowName,
        WINDOW_STYLES    dwStyle,
        int              x,
        int              y,
        int              nWidth,
        int              nHeight,
        HWND             hWndParent,
        HMENU            hMenu,
        HINSTANCE        hInstance,
        in T             param
    ) where T : unmanaged
    {
        fixed (char* lpClassName  = className)
        fixed (char* lpWindowName = windowName)
        fixed (void* lpParam      = &param)
        {
            return CreateWindowExW(
                dwExStyle,
                lpClassName,
                lpWindowName,
                dwStyle,
                x,
                y,
                nWidth,
                nHeight,
                hWndParent,
                hMenu,
                hInstance,
                (LPVOID)lpParam
            );
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-closeclipboard"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL CloseClipboard();

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-defwindowprocw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial LRESULT DefWindowProcW(HWND hWnd, WINDOW_MESSAGE uMsg, WPARAM wParam, LPARAM lParam);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-destroywindow"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL DestroyWindow(HWND hWnd);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-dispatchmessagew"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial LRESULT DispatchMessageW(in MSG lpMsg);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-endpaint"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL EndPaint(HWND hWnd, PAINTSTRUCT* lpPaint);

    /// <inheritdoc cref="EndPaint" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL EndPaint(HWND hWnd, in PAINTSTRUCT lpPaint);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaymonitors"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL EnumDisplayMonitors(HDC hdc, LPCRECT lprcClip, MONITORENUMPROC lpfnEnum, LPARAM dwData);

    /// <inheritdoc cref="EnumDisplayMonitors" />
    public static BOOL EnumDisplayMonitors<T>(HDC hdc, in RECT clip, MONITORENUMPROC lpfnEnum, ref T data) where T : unmanaged
    {
        fixed (RECT* lprcClip = &clip)
        fixed (T*    dwData   = &data)
        {
            return EnumDisplayMonitors(hdc, clip.Equals(default(RECT)) ? default : (LPCRECT)lprcClip, lpfnEnum, (LPARAM)dwData);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-emptyclipboard"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL EmptyClipboard();

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-fillrect"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial int FillRect(HDC hDC, RECT* lprc, HBRUSH hbr);

    /// <inheritdoc cref="FillRect" />
    [LibraryImport(nameof(User32))]
    public static partial int FillRect(HDC hDC, in RECT lprc, HBRUSH hbr);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclientrect"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetClientRect(HWND hWnd, LPRECT lpRect);

    /// <inheritdoc cref="GetClientRect" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetClientRect(HWND hWnd, out RECT lpRect);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getclipboarddata"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HANDLE GetClipboardData(STANDARD_CLIPBOARD_FORMATS uFormat);

    /// <inheritdoc cref="GetClipboardData" />
    public static string? GetClipboardTextData() =>
        Marshal.PtrToStringAnsi(GetClipboardData(STANDARD_CLIPBOARD_FORMATS.CF_TEXT));

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getcursorpos"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetCursorPos(LPPOINT lpPoint);

    /// <inheritdoc cref="GetCursorPos" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetCursorPos(out POINT lpPoint);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdc"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HDC GetDC(HWND hWnd);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeystate"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial SHORT GetKeyState(VIRTUAL_KEYS virtualKey);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmessageextrainfo"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial LPARAM GetMessageExtraInfo();

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmonitorinfow"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetMonitorInfoW(HMONITOR hMonitor, LPMONITORINFO lpmi);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputdata"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial UINT GetRawInputData(HRAWINPUT hRawInput, RAW_INPUT_DATA_COMMAND_FLAGS uiCommand, LPVOID pData, PUINT pcbSize, UINT cbSizeHeader);

    /// <inheritdoc cref="GetRawInputData" />
    public static uint GetRawInputData<T>(ref RAWINPUT rawInput, RAW_INPUT_DATA_COMMAND_FLAGS uiCommand, ref T data, ref UINT size, UINT cbSizeHeader) where T : unmanaged
    {
        fixed (RAWINPUT* pRawInput = &rawInput)
        fixed (void*     pdata     = &data)
        fixed (UINT*     pcbSize   = &size)
        {
            return GetRawInputData((HRAWINPUT)pRawInput, uiCommand, (LPVOID)pdata, (PUINT)pcbSize, cbSizeHeader);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial int GetSystemMetrics(SYSTEM_METRIC nIndex);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getupdaterect"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetUpdateRect(HWND hWnd, LPRECT lpRect, BOOL bErase);

    /// <summary>
    /// See <see href="GetUpdateRect"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetUpdateRect(HWND hWnd, out RECT rect, BOOL bErase);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptrw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR GetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowplacement"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowPlacement(HWND hWnd, WINDOWPLACEMENT *lpwndpl);

    /// <inheritdoc cref="GetWindowPlacement" />
    public static BOOL GetWindowPlacement(HWND hWnd, out WINDOWPLACEMENT wndpl)
    {
        fixed (WINDOWPLACEMENT* lpwndpl = &wndpl)
        {
            return GetWindowPlacement(hWnd, lpwndpl);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowRect(HWND hWnd, LPRECT lpRect);

    /// <inheritdoc cref="GetWindowRect" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowRect(HWND hWnd, out RECT lpRect);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-iswindow"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL IsWindow(HWND hWnd);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-iswindowvisible"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL IsWindowVisible(HWND hWnd);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-killtimer"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL KillTimer(HWND hWnd, UINT_PTR uIDEvent);

    /// <inheritdoc cref="KillTimer" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL KillTimer(HWND hWnd, ref UINT uIDEvent);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-loadcursorw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HCURSOR LoadCursorW(HINSTANCE hInstance, LPCWSTR lpCursorName);

    /// <inheritdoc cref="LoadCursorW(HINSTANCE, LPCWSTR)"/>
    [LibraryImport(nameof(User32))]
    public static partial HCURSOR LoadCursorW(HINSTANCE hInstance, IDC_STANDARD_CURSORS lpCursorName);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messageboxw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial int MessageBoxW(HWND hWnd, LPCWSTR lpText, LPCWSTR lpCaption, MESSAGE_BOX_OPTIONS uType);

    /// <inheritdoc cref="MessageBoxW" />
    public static int MessageBoxW(HWND hWnd, string text, string caption, MESSAGE_BOX_OPTIONS type)
    {
        fixed (char* lpText    = text)
        fixed (char* lpCaption = caption)
        {
            return MessageBoxW(hWnd, lpText, lpCaption, type);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfromwindow"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HMONITOR MonitorFromWindow(HWND hwnd, DWORD dwFlags);

    /// <inheritdoc cref="MonitorFromWindow" />
    [LibraryImport(nameof(User32))]
    public static partial HMONITOR MonitorFromWindow(HWND hwnd, MONITOR dwFlags);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-movewindow"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL MoveWindow(HWND hWnd, int x, int y, int nWidth, int nHeight, BOOL bRepaint);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-openclipboard"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL OpenClipboard(HWND hWndNewOwner);

    /// <summary>
    /// See <see href="">https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-peekmessagew</see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL PeekMessageW(LPMSG lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT wMsgFilterMax, PEEK_MESSAGE wRemoveMsg);

    /// <inheritdoc cref="PeekMessageW(LPMSG, HWND, UINT, UINT, PEEK_MESSAGE)"/>
    [LibraryImport(nameof(User32))]
    public static partial BOOL PeekMessageW(out MSG lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT wMsgFilterMax, PEEK_MESSAGE wRemoveMsg);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerclassexw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial ATOM RegisterClassExW(in WNDCLASSEXW lpwcx);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-releasecapture"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL ReleaseCapture();

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-releasedc"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial int ReleaseDC(HWND hWnd, HDC hDC);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-screentoclient"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL ScreenToClient(HWND hWnd, LPPOINT lpPoint);

    /// <inheritdoc cref="ScreenToClient" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL ScreenToClient(HWND hWnd, in POINT lpPoint);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcapture"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HWND SetCapture(HWND hWnd);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setclipboarddata"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HANDLE SetClipboardData(STANDARD_CLIPBOARD_FORMATS uFormat, HANDLE hMem);

    /// <inheritdoc cref="SetClipboardData" />
    public static HANDLE SetClipboardData(string value) =>
        SetClipboardData(STANDARD_CLIPBOARD_FORMATS.CF_TEXT, Marshal.StringToHGlobalAnsi(value));

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursor"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HCURSOR SetCursor(HCURSOR handle);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setcursorpos"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL SetCursorPos(int x, int y);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setfocus"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HWND SetFocus(HWND hWnd);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL SetForegroundWindow(HWND hWnd);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-settimer"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial UINT_PTR SetTimer(HWND hWnd, UINT_PTR nIDEvent, UINT uElapse, TIMERPROC lpTimerFunc);

    /// <inheritdoc cref="SetTimer" />
    [LibraryImport(nameof(User32))]
    public static partial UINT_PTR SetTimer(HWND hWnd, in UINT nIDEvent, UINT uElapse, TIMERPROC lpTimerFunc);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptrw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex, LONG_PTR dwNewLong);

    /// <inheritdoc cref="SetWindowLongPtrW" />
    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex, in LONG dwNewLong);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL SetWindowPos(HWND hWnd, WINDOW_ZORDER hWndInsertAfter, int x, int y, int cx, int cy, WINDOW_POS_FLAGS uFlags);

    /// <inheritdoc cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowrgn" />
    [LibraryImport(nameof(User32))]
    public static partial int SetWindowRgn(HWND hWnd, HRGN hRgn, BOOL bRedraw);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HHOOK SetWindowsHookExW(WINDOWS_HOOK_TYPE hookType, HOOKPROC lpfn, HINSTANCE hMod, DWORD dwThreadId);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowtextw"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL SetWindowTextW(HWND hWnd, LPCWSTR lpString);

    /// <inheritdoc cref="SetWindowTextW" />
    public static BOOL SetWindowText(HWND hWnd, string? text)
    {
        fixed (char* lpString = text)
        {
            return SetWindowTextW(hWnd, lpString);
        }
    }

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL ShowWindow(HWND hWnd, SHOW_WINDOW_COMMANDS nCmdShow);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-trackmouseevent"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL TrackMouseEvent(LPTRACKMOUSEEVENT lpEventTrack);

    /// <inheritdoc cref="TrackMouseEvent" />
    [LibraryImport(nameof(User32))]
    public static partial BOOL TrackMouseEvent(in TRACKMOUSEEVENT eventTrack);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-translatemessage"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial BOOL TranslateMessage(in MSG lpMsg);

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-windowfrompoint"></see>
    /// </summary>
    [LibraryImport(nameof(User32))]
    public static partial HWND WindowFromPoint(POINT point);
}
