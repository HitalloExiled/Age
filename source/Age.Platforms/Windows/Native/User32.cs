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

    /// <summary>Deactivated.</summary>
    public const int WA_INACTIVE = 0;

    /// <summary>
    /// Activated by some method other than a mouse click (for example, by a call to
    /// the SetActiveWindow function or by use of the keyboard interface to select the window).
    /// </summary>
    public const int WA_ACTIVE = 1;

    /// <summary>Activated by a mouse click.</summary>
    public const int WA_CLICKACTIVE = 2;

    [LibraryImport(nameof(User32))]
    public static partial BOOL AdjustWindowRectEx(LPRECT lpRect, WINDOW_STYLES dwStyle, BOOL bMenu, WINDOW_STYLES_EX dwExStyle);

    [LibraryImport(nameof(User32))]
    public static partial BOOL AdjustWindowRectEx(ref RECT lpRect, WINDOW_STYLES dwStyle, BOOL bMenu, WINDOW_STYLES_EX dwExStyle);

    [LibraryImport(nameof(User32))]
    public static partial HDC BeginPaint(HWND hWnd, LPPAINTSTRUCT lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial HDC BeginPaint(HWND hWnd, out PAINTSTRUCT lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial LRESULT CallNextHookEx(HHOOK hhk, int nCode, WINDOW_MESSAGE wParam, LPARAM lParam);

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

    [LibraryImport(nameof(User32))]
    public static partial BOOL ClipCursor(RECT* lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ClipCursor(in RECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial HRGN CreatePolygonRgn(POINT* pptl, int cPoint, FILL_MODE iMode);

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

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createwindowexw"></see>
    /// </summary>
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

    [LibraryImport(nameof(User32))]
    public static partial BOOL EndPaint(HWND hWnd, PAINTSTRUCT* lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL EndPaint(HWND hWnd, in PAINTSTRUCT lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL EnumDisplayMonitors(HDC hdc, LPCRECT lprcClip, MONITORENUMPROC lpfnEnum, LPARAM dwData);

    public static BOOL EnumDisplayMonitors<T>(HDC hdc, in RECT clip, MONITORENUMPROC lpfnEnum, ref T data) where T : unmanaged
    {
        fixed (RECT* lprcClip = &clip)
        fixed (T*    dwData   = &data)
        {
            return EnumDisplayMonitors(hdc, clip.Equals(default(RECT)) ? default : (LPCRECT)lprcClip, lpfnEnum, (LPARAM)dwData);
        }
    }

    [LibraryImport(nameof(User32))]
    public static partial int FillRect(HDC hDC, RECT* lprc, HBRUSH hbr);

    [LibraryImport(nameof(User32))]
    public static partial int FillRect(HDC hDC, in RECT lprc, HBRUSH hbr);

    /// <summary>
    /// Retrieves the coordinates of a window's client area. The client coordinates specify the upper-left and lower-right corners of the client area. Because client coordinates are relative to the upper-left corner of a window's client area, the coordinates of the upper-left corner are (0,0).
    /// </summary>
    /// <param name="hWnd">A handle to the window whose client coordinates are to be retrieved.</param>
    /// <param name="lpRect">A pointer to a <see cref="RECT"/> structure that receives the client coordinates. The left and top members are zero. The right and bottom members contain the width and height of the window.</param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero. To get extended error information, call <see cref="Kernel32.GetLastError"/>.
    /// </returns>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetClientRect(HWND hWnd, LPRECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetClientRect(HWND hWnd, out RECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetCursorPos(LPPOINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetCursorPos(out POINT lpPoint);

    /// <summary>
    /// <para>The <see cref="GetDC"/> function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.
    /// You can use the returned handle in subsequent GDI functions to draw in the DC. The device context is an opaque data structure, whose values are used internally by GDI.</para>
    /// <para>The <see cref="GetDCEx"/> function is an extension to GetDC, which gives an application more control over how and whether clipping occurs in the client area.</para>
    /// </summary>
    /// <param name="hWnd">A handle to the window whose DC is to be retrieved. If this value is NULL, GetDC retrieves the DC for the entire screen.</param>
    /// <returns>
    /// <para>If the function succeeds, the return value is a handle to the DC for the specified window's client area.</para>
    /// <para>If the function fails, the return value is NULL.</para>
    /// </returns>
    /// <remarks>
    /// <para>The GetDC function retrieves a common, class, or private DC depending on the class style of the specified window. For class and private DCs,
    /// GetDC leaves the previously assigned attributes unchanged. However, for common DCs, GetDC assigns default attributes to the DC each time it is retrieved. For example,
    /// the default font is System, which is a bitmap font. Because of this, the handle to a common DC returned by GetDC does not tell you what font, color, or brush was used when the window was drawn.
    /// To determine the font, call GetTextFace.</para>
    /// <para>Note that the handle to the DC can only be used by a single thread at any one time.</para>
    /// <para>After painting with a common DC, the ReleaseDC function must be called to release the DC. Class and private DCs do not have to be released.
    /// ReleaseDC must be called from the same thread that called GetDC. The number of DCs is limited only by available memory.</para>
    /// </remarks>
    /// <example>
    /// For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/gdi/drawing-with-the-mouse">Drawing with the Mouse</see>.
    /// </example>
    [LibraryImport(nameof(User32))]
    public static partial HDC GetDC(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial SHORT GetKeyState(VIRTUAL_KEYS virtualKey);

    [LibraryImport(nameof(User32))]
    public static partial LPARAM GetMessageExtraInfo();

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetMonitorInfoW(HMONITOR hMonitor, LPMONITORINFO lpmi);

    [LibraryImport(nameof(User32))]
    public static partial UINT GetRawInputData(HRAWINPUT hRawInput, RAW_INPUT_DATA_COMMAND_FLAGS uiCommand, LPVOID pData, PUINT pcbSize, UINT cbSizeHeader);

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

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetUpdateRect(HWND hWnd, LPRECT lpRect, BOOL bErase);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetUpdateRect(HWND hWnd, out RECT rect, BOOL bErase);

    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR GetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex);

    /// <summary>
    /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="*lpwndpl">A pointer to the <see cref="WINDOWPLACEMENT"/> structure that receives the show state and position information. Before calling <see cref="GetWindowPlacement"/>, set the length member to sizeof(WINDOWPLACEMENT). <see cref="GetWindowPlacement"/> fails if lpwndpl-> length is not set correctly.</param>
    /// <returns></returns>
    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowPlacement(HWND hWnd, WINDOWPLACEMENT *lpwndpl);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static BOOL GetWindowPlacement(HWND hWnd, out WINDOWPLACEMENT wndpl)
    {
        fixed (WINDOWPLACEMENT* lpwndpl = &wndpl)
        {
            return GetWindowPlacement(hWnd, lpwndpl);
        }
    }

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowRect(HWND hWnd, LPRECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowRect(HWND hWnd, out RECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL IsWindow(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial BOOL IsWindowVisible(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial BOOL KillTimer(HWND hWnd, UINT_PTR uIDEvent);

    [LibraryImport(nameof(User32))]
    public static partial BOOL KillTimer(HWND hWnd, ref UINT uIDEvent);

    /// <summary>
    /// Loads the specified cursor resource from the executable (.exe) file associated with an application instance.
    /// </summary>
    /// <param name="hInstance">
    /// <para>A handle to the module of either a DLL or executable (.exe) file that contains the cursor to be loaded. For more information, see <see cref="GetModuleHandle"/>.</para>
    /// <para>To load a predefined system cursor, set this parameter to NULL.</para>
    /// </param>
    /// <param name="lpCursorName">
    /// <para>If hInstance is non-NULL, lpCursorName specifies the cursor resource either by name or ordinal.</para>
    /// <para>If hInstance is NULL, lpCursorName specifies the identifier begin with the IDC_ prefix of a predefined system cursor to load.</para>
    /// </param>
    /// <returns>
    /// <para>If the function succeeds, the return value is the handle to the newly loaded cursor.</para>
    /// <para>If the function fails, the return value is NULL. To get extended error information, call <see cref="Kernel32.GetLastError"/>.</para>
    /// </returns>
    /// <remarks>
    /// <para>The <see cref="LoadCursorW"/> function loads the cursor resource only if it has not been loaded; otherwise, it retrieves the handle to the existing resource. This function returns a valid cursor handle only if the lpCursorName parameter is a pointer to a cursor resource. If lpCursorName is a pointer to any type of resource other than a cursor (such as an icon), the return value is not NULL, even though it is not a valid cursor handle.</para>
    /// <para>The <see cref="LoadCursorW"/> function searches the cursor resource most appropriate for the cursor for the current display device. The cursor resource can be a color or monochrome bitmap.</para>
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/menurc/using-cursors">Creating a Cursor</see>.</example>
    [LibraryImport(nameof(User32))]
    public static partial HCURSOR LoadCursorW(HINSTANCE hInstance, LPCWSTR lpCursorName);

    /// <inheritdoc cref="LoadCursorW(HINSTANCE, LPCWSTR)"/>
    [LibraryImport(nameof(User32))]
    public static partial HCURSOR LoadCursorW(HINSTANCE hInstance, IDC_STANDARD_CURSORS lpCursorName);

    [LibraryImport(nameof(User32))]
    public static partial int MessageBoxW(HWND hWnd, LPCWSTR lpText, LPCWSTR lpCaption, MESSAGE_BOX_OPTIONS uType);

    public static int MessageBoxW(HWND hWnd, string text, string caption, MESSAGE_BOX_OPTIONS type)
    {
        fixed (char* lpText    = text)
        fixed (char* lpCaption = caption)
        {
            return MessageBoxW(hWnd, lpText, lpCaption, type);
        }
    }

    [LibraryImport(nameof(User32))]
    public static partial HMONITOR MonitorFromWindow(HWND hwnd, DWORD dwFlags);

    [LibraryImport(nameof(User32))]
    public static partial HMONITOR MonitorFromWindow(HWND hwnd, MONITOR dwFlags);

    [LibraryImport(nameof(User32))]
    public static partial BOOL MoveWindow(HWND hWnd, int x, int y, int nWidth, int nHeight, BOOL bRepaint);

    /// <summary>
    /// Dispatches incoming nonqueued messages, checks the thread message queue for a posted message, and retrieves the message (if any exist).
    /// </summary>
    /// <param name="lpMsg">
    /// <para>A pointer to an MSG structure that receives message information</para>
    /// <para>If hWnd is NULL, PeekMessage retrieves messages for any window that belongs to the current thread, and any messages on the current thread's message queue whose hwnd value is NULL (see the MSG structure).
    /// Therefore if hWnd is NULL, both window messages and thread messages are processed.</para>
    /// <para>If hWnd is -1, PeekMessage retrieves only messages on the current thread's message queue whose hwnd value is NULL, that is,
    /// thread messages as posted by PostMessage (when the hWnd parameter is NULL) or PostThreadMessage.</para>
    /// </param>
    /// <param name="hWnd">A handle to the window whose messages are to be retrieved. The window must belong to the current thread.</param>
    /// <param name="wMsgFilterMin">
    /// <para>The value of the first message in the range of messages to be examined. Use WM_KEYFIRST (0x0100) to specify the first keyboard message or WM_MOUSEFIRST (0x0200) to specify the first mouse message.</para>
    /// <para>If wMsgFilterMin and wMsgFilterMax are both zero, PeekMessage returns all available messages (that is, no range filtering is performed).</para>
    /// </param>
    /// <param name="wMsgFilterMax">
    /// <para>The value of the last message in the range of messages to be examined. Use WM_KEYLAST to specify the last keyboard message or WM_MOUSELAST to specify the last mouse message.</para>
    /// <para>If wMsgFilterMin and wMsgFilterMax are both zero, PeekMessage returns all available messages (that is, no range filtering is performed).</para>
    /// </param>
    /// <param name="wRemoveMsg">Specifies how messages are to be handled.</param>
    /// <returns>
    /// <para>If a message is available, the return value is nonzero.</para>
    /// <para>If no messages are available, the return value is zero.</para>
    /// </returns>
    /// <remarks>
    /// <para>
    /// PeekMessage retrieves messages associated with the window identified by the hWnd parameter or any of its children as specified by the IsChild function,
    /// and within the range of message values given by the wMsgFilterMin and wMsgFilterMax parameters.</para>
    /// <para>Note that an application can only use the low word in the wMsgFilterMin and wMsgFilterMax parameters; the high word is reserved for the system.</para>
    /// <para>Note that PeekMessage always retrieves WM_QUIT messages, no matter which values you specify for wMsgFilterMin and wMsgFilterMax.</para>
    /// <para>During this call, the system dispatches (DispatchMessage) pending, nonqueued messages, that is, messages sent to windows owned by the calling thread using the SendMessage,
    /// SendMessageCallback, SendMessageTimeout, or SendNotifyMessage function. Then the first queued message that matches the specified filter is retrieved.
    /// The system may also process internal events. If no filter is specified, messages
    /// are processed in the following order:</para>
    /// <list type="bullet">
    /// <item>Sent messages</item>
    /// <item>Posted messages</item>
    /// <item>Input (hardware) messages and system internal events</item>
    /// <item>Sent messages (again)</item>
    /// <item>WM_PAINT messages</item>
    /// <item>WM_TIMER messages</item>
    /// </list>
    /// <para>To retrieve input messages before posted messages, use the wMsgFilterMin and wMsgFilterMax parameters.</para>
    /// <para>The PeekMessage function normally does not remove WM_PAINT messages from the queue. WM_PAINT messages remain in the queue
    /// until they are processed. However, if a WM_PAINT message has a NULL update region, PeekMessage does remove it from the queue.</para>
    /// <para>If a top-level window stops responding to messages for more than several seconds, the system considers the window to be not
    /// responding and replaces it with a ghost window that has the same z-order, location, size, and visual attributes. This allows the user
    /// to move it, resize it, or even close the application. However, these are the only actions available because the application is actually
    /// not responding. When an application is being debugged, the system does not generate a ghost window.</para>
    /// </remarks>
    /// <example>
    /// For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/using-messages-and-message-queues">Examining a Message Queue</see>.
    /// </example>
    [LibraryImport(nameof(User32))]
    public static partial BOOL PeekMessageW(LPMSG lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT wMsgFilterMax, PEEK_MESSAGE wRemoveMsg);

    /// <inheritdoc cref="PeekMessageW(LPMSG, HWND, UINT, UINT, PEEK_MESSAGE)"/>
    [LibraryImport(nameof(User32))]
    public static partial BOOL PeekMessageW(out MSG lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT wMsgFilterMax, PEEK_MESSAGE wRemoveMsg);

    /// <summary>
    /// Registers a window class for subsequent use in calls to the <see cref="CreateWindow"/> or <see cref="CreateWindowEx"/> function.
    /// </summary>
    /// <param name="lpwcx">A pointer to a <see cref="WNDCLASSEXW"/> structure. You must fill the structure with the appropriate class attributes before passing it to the function.</param>
    /// <returns>
    /// <para>
    /// If the function succeeds, the return value is a class atom that uniquely identifies the class being registered.
    /// This atom can only be used by the <see cref="CreateWindow"/>, <see cref="CreateWindowEx"/>, <see cref="GetClassInfo"/>, <see cref="GetClassInfoEx"/>, <see cref="FindWindow"/>, <see cref="FindWindowEx"/>,
    /// and <see cref="UnregisterClass"/> functions and the IActiveIMMap::FilterClientWindows method.
    /// </para>
    /// <para>If the function fails, the return value is zero. To get extended error information, call <see cref="Kernel32.GetLastError"/>.</para>
    /// </returns>
    /// <remarks>
    /// <para>
    /// If you register the window class by using <see cref="RegisterClassExA"/>, the application tells the system that the windows of the created class expect messages with text or character parameters to use the ANSI character set;
    /// if you register it by using <see cref="RegisterClassExW"/>, the application requests that the system pass text parameters of messages as Unicode. The IsWindowUnicode function enables applications to query the nature of each window.
    /// For more information on ANSI and Unicode functions, see Conventions for Function Prototypes.
    /// </para>
    /// <para>All window classes that an application registers are unregistered when it terminates.</para>
    /// <para>No window classes registered by a DLL are unregistered when the DLL is unloaded. A DLL must explicitly unregister its classes when it is unloaded.</para>
    /// </remarks>
    /// <example>
    /// For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/using-window-classes">Using Window Classes</see>.
    /// </example>
    [LibraryImport(nameof(User32))]
    public static partial ATOM RegisterClassExW(in WNDCLASSEXW lpwcx);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ReleaseCapture();

    /// <summary>
    /// The ReleaseDC function releases a device context (DC), freeing it for use by other applications. The effect of the ReleaseDC function depends on the type of DC. It frees only common and window DCs. It has no effect on class or private DCs.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
    /// <param name="hDC">A handle to the DC to be released.</param>
    /// <returns>
    /// <para>The return value indicates whether the DC was released. If the DC was released, the return value is 1.</para>
    /// <para>If the DC was not released, the return value is zero.</para>
    /// </returns>
    /// <remarks>
    /// <para>The application must call the ReleaseDC function for each call to the GetWindowDC function and for each call to the GetDC function that retrieves a common DC.</para>
    /// <para>
    /// An application cannot use the ReleaseDC function to release a DC that was created by calling the CreateDC function; instead, it must use the DeleteDC function.
    /// ReleaseDC must be called from the same thread that called GetDC.
    /// </para>
    /// </remarks>
    /// <example>
    /// For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/gdi/scaling-an-image">Scaling an Image.</see>.
    /// </example>
    [LibraryImport(nameof(User32))]
    public static partial int ReleaseDC(HWND hWnd, HDC hDC);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ScreenToClient(HWND hWnd, LPPOINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ScreenToClient(HWND hWnd, in POINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial HWND SetCapture(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial HCURSOR SetCursor(HCURSOR handle);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetCursorPos(int x, int y);

    [LibraryImport(nameof(User32))]
    public static partial HWND SetFocus(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetForegroundWindow(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial UINT_PTR SetTimer(HWND hWnd, UINT_PTR nIDEvent, UINT uElapse, TIMERPROC lpTimerFunc);

    [LibraryImport(nameof(User32))]
    public static partial UINT_PTR SetTimer(HWND hWnd, in UINT nIDEvent, UINT uElapse, TIMERPROC lpTimerFunc);

    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex, LONG_PTR dwNewLong);

    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex, in LONG dwNewLong);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetWindowPos(HWND hWnd, WINDOW_ZORDER hWndInsertAfter, int x, int y, int cx, int cy, WINDOW_POS_FLAGS uFlags);

    [LibraryImport(nameof(User32))]
    public static partial int SetWindowRgn(HWND hWnd, HRGN hRgn, BOOL bRedraw);

    [LibraryImport(nameof(User32))]
    public static partial HHOOK SetWindowsHookExW(WINDOWS_HOOK_TYPE hookType, HOOKPROC lpfn, HINSTANCE hMod, DWORD dwThreadId);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetWindowTextW(HWND hWnd, LPCWSTR lpString);

    public static BOOL SetWindowTextW(HWND hWnd, string? text)
    {
        fixed (char* lpString = text)
        {
            return SetWindowTextW(hWnd, lpString);
        }
    }

    /// <summary>
    /// Sets the specified window's show state.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="nCmdShow">
    /// Controls how the window is to be shown. This parameter is ignored the first time an application calls <see cref="ShowWindow"/>,
    /// if the program that launched the application provides a <see cref="STARTUPINFO"/> structure. Otherwise, the first time <see cref="ShowWindow"/> is called,
    /// the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls,
    /// this parameter can be one of the following values in <see cref="SHOW_WINDOW_COMMANDS"/>.
    /// </param>
    /// <returns>
    /// <para>If the window was previously visible, the return value is nonzero.</para>
    /// <para>If the window was previously hidden, the return value is zero.</para>
    /// </returns>
    /// <remarks>
    /// <para>To perform certain special effects when showing or hiding a window, use AnimateWindow.</para>
    /// <para>The first time an application calls ShowWindow, it should use the WinMain function's nCmdShow parameter as its nCmdShow parameter. Subsequent calls to <see cref="ShowWindow"/> must use one of the values in the given list, instead of the one specified by the WinMain function's nCmdShow parameter.</para>
    /// <para>
    /// As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call to ShowWindow if the program that launched the application specifies startup information in the structure. In this case, <see cref="ShowWindow"/> uses the information specified in the <see cref="STARTUPINFO"/> structure to show the window. On subsequent calls,
    /// the application must call <see cref="ShowWindow"/> with nCmdShow set to <see cref="SW_SHOWDEFAULT"/> to use the startup information provided by the program that launched the application. This behavior is designed for the following situations:
    /// <list type="bullet">
    /// <item>Applications create their main window by calling CreateWindow with the <see cref="WINDOW_STYLES.WS_VISIBLE"/> flag set.</item>
    /// <item>Applications create their main window by calling CreateWindow with the <see cref="WINDOW_STYLES.WS_VISIBLE"/> flag cleared, and later call <see cref="ShowWindow"/> with the <see cref="SHOW_WINDOW_COMMANDS.SW_SHOW"/> flag set to make it visible.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/using-windows">Creating a Main Window</see>.</example>
    [LibraryImport(nameof(User32))]
    public static partial BOOL ShowWindow(HWND hWnd, SHOW_WINDOW_COMMANDS nCmdShow);

    [LibraryImport(nameof(User32))]
    public static partial BOOL TrackMouseEvent(LPTRACKMOUSEEVENT lpEventTrack);

    [LibraryImport(nameof(User32))]
    public static partial BOOL TrackMouseEvent(in TRACKMOUSEEVENT eventTrack);

    /// <summary>
    /// Translates virtual-key messages into character messages. The character messages are posted to the calling thread's message queue, to be read the next time the thread calls the <see cref="GetMessage"/> or <see cref="PeekMessage"/> function.
    /// </summary>
    /// <param name="lpMsg">A pointer to an <see cref="MSG"/> structure that contains message information retrieved from the calling thread's message queue by using the <see cref="GetMessage"/> or <see cref="PeekMessage"/> function.</param>
    /// <returns>
    /// <para>If the message is translated (that is, a character message is posted to the thread's message queue), the return value is nonzero.</para>
    /// <para>If the message is <see cref="WINDOW_MESSAGE.WM_KEYDOWN"/>, <see cref="WINDOW_MESSAGE.WM_KEYUP"/>, <see cref="WINDOW_MESSAGE.WM_SYSKEYDOWN"/>, or <see cref="WINDOW_MESSAGE.WM_SYSKEYUP"/>, the return value is nonzero, regardless of the translation.</para>
    /// <para>If the message is not translated (that is, a character message is not posted to the thread's message queue), the return value is zero.</para>
    /// </returns>
    /// <remarks>
    /// The TranslateMessage function does not modify the message pointed to by the lpMsg parameter.
    /// <see cref="WINDOW_MESSAGE.WM_KEYDOWN"/> and <see cref="WINDOW_MESSAGE.WM_KEYUP"/> combinations produce a <see cref="WINDOW_MESSAGE.WM_CHAR"/> or <see cref="WINDOW_MESSAGE.WM_DEADCHAR"/> message. <see cref="WM_SYSKEYDOWN"/> and <see cref="WINDOW_MESSAGE.WM_SYSKEYUP"/> combinations produce a <see cref="WINDOW_MESSAGE.WM_SYSCHAR"/> or <see cref="WM_SYSDEADCHAR"/> message.
    /// TranslateMessage produces <see cref="WINDOW_MESSAGE.WM_CHAR"/> messages only for keys that are mapped to ASCII characters by the keyboard driver.
    /// If applications process virtual-key messages for some other purpose, they should not call TranslateMessage. For instance, an application should not call TranslateMessage if the TranslateAccelerator function returns a nonzero value. Note that the application is responsible for retrieving and dispatching input messages to the dialog box. Most applications use the main message loop for this. However, to permit the user to move to and to select controls by using the keyboard, the application must call IsDialogMessage. For more information, see Dialog Box Keyboard Interface.
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/using-messages-and-message-queues">Creating a Message Loop</see>.</example>
    [LibraryImport(nameof(User32))]
    public static partial BOOL TranslateMessage(in MSG lpMsg);

    [LibraryImport(nameof(User32))]
    public static partial HWND WindowFromPoint(POINT point);
}
