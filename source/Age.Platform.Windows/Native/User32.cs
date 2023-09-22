using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Platform.Windows.Native.Types;

[assembly: InternalsVisibleTo("Age.Platform.Windows.Display")]

namespace Age.Platform.Windows.Native;

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
    /// The ClientToScreen function converts the client-area coordinates of a specified point to screen coordinates.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose client area is used for the conversion.</param>
    /// <param name="lpPoint">A pointer to a <see cref="POINT"/> structure that contains the client coordinates to be converted. The new screen coordinates are copied into this structure if the function succeeds.</param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.
    /// </returns>
    /// <remarks>
    /// <para>The <see cref="ClientToScreen"/> function replaces the client-area coordinates in the <see cref="POINT"/> structure with the screen coordinates. The screen coordinates are relative to the upper-left corner of the screen. Note, a screen-coordinate point that is above the window's client area has a negative y-coordinate. Similarly, a screen coordinate to the left of a client area has a negative x-coordinate.</para>
    /// <para>All coordinates are device coordinates.</para>
    /// </remarks>
    [LibraryImport(nameof(User32))]
    public static partial BOOL ClientToScreen(HWND hWnd, LPPOINT lpPoint);

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
    /// Creates an overlapped, pop-up, or child window with an extended window style; otherwise, this function is identical to the CreateWindow function.
    /// For more information about creating a window and for full descriptions of the other parameters of <see cref="CreateWindowEx"/>, see <see cref="CreateWindow"/>.
    /// </summary>
    /// <param name="dwExStyle">The extended window style of the window being created.</param>
    /// <param name="lpClassName">
    /// A null-terminated string or a class atom created by a previous call to the <see cref="RegisterClass"/> or <see cref="RegisterClassEx"/> function.
    /// The atom must be in the low-order word of lpClassName; the high-order word must be zero. If lpClassName is a string, it specifies the window class name.
    /// The class name can be any name registered with <see cref="RegisterClass"/> or <see cref="RegisterClassEx"/>, provided that the module that registers the class is also the module that creates the window.
    /// The class name can also be any of the predefined system class names.
    /// </param>
    /// <param name="lpWindowName">
    /// The window name. If the window style specifies a title bar, the window title pointed to by lpWindowName is displayed in the title bar.
    /// When using CreateWindow to create controls, such as buttons, check boxes, and static controls, use lpWindowName to specify the text of the control. When creating a static control with the SS_ICON style,
    /// use lpWindowName to specify the icon name or identifier. To specify an identifier, use the syntax "#num".
    /// </param>
    /// <param name="dwStyle">The style of the window being created. This parameter can be a combination of the window style values, plus the control styles indicated in the Remarks section.</param>
    /// <param name="x">
    /// <para>The initial horizontal position of the window. For an overlapped or pop-up window, the x parameter is the initial x-coordinate of the window's upper-left corner, in screen coordinates.
    /// For a child window, x is the x-coordinate of the upper-left corner of the window relative to the upper-left corner of the parent window's client area. If x is set to <see cref="CW_USEDEFAULT"/>,
    /// the system selects the default position for the window's upper-left corner and ignores the y parameter. <see cref="CW_USEDEFAULT"/> is valid only for overlapped windows; if it is specified for a pop-up or child window,
    /// the x and y parameters are set to zero.</para>
    /// <para>If an overlapped window is created with the WS_VISIBLE style bit set and the x parameter is set to <see cref="CW_USEDEFAULT"/>, then the y parameter determines how the window is shown. If the y parameter is <see cref="CW_USEDEFAULT"/>,
    /// then the window manager calls ShowWindow with the SW_SHOW flag after the window has been created. If the y parameter is some other value, then the window manager calls ShowWindow with that value as the nCmdShow parameter.</para>
    /// </param>
    /// <param name="y">The initial vertical position of the window. For an overlapped or pop-up window, the y parameter is the initial y-coordinate of the window's upper-left corner, in screen coordinates. For a child window,
    /// y is the initial y-coordinate of the upper-left corner of the child window relative to the upper-left corner of the parent window's client area. For a list box y is the initial y-coordinate of the upper-left corner of the list box's client area relative to the upper-left corner of the parent window's client area.</param>
    /// <param name="nWidth">The width, in device units, of the window. For overlapped windows, nWidth is the window's width, in screen coordinates, or <see cref="CW_USEDEFAULT"/>. If nWidth is <see cref="CW_USEDEFAULT"/>, the system selects a default width and height for the window; the default width extends from the initial x-coordinates to the right edge of the screen;
    /// the default height extends from the initial y-coordinate to the top of the icon area. <see cref="CW_USEDEFAULT"/> is valid only for overlapped windows; if <see cref="CW_USEDEFAULT"/> is specified for a pop-up or child window, the nWidth and nHeight parameter are set to zero.</param>
    /// <param name="nHeight">The height, in device units, of the window. For overlapped windows, nHeight is the window's height, in screen coordinates. If the nWidth parameter is set to <see cref="CW_USEDEFAULT"/>, the system ignores nHeight.</param>
    /// <param name="hWndParent">
    /// <para>A handle to the parent or owner window of the window being created. To create a child window or an owned window, supply a valid window handle. This parameter is optional for pop-up windows.</para>
    /// <para>To create a message-only window, supply HWND_MESSAGE or a handle to an existing message-only window.</para>
    /// </param>
    /// <param name="hMenu">A handle to a menu, or specifies a child-window identifier, depending on the window style. For an overlapped or pop-up window, hMenu identifies the menu to be used with the window; it can be NULL if the class menu is to be used. For a child window,
    /// hMenu specifies the child-window identifier, an integer value used by a dialog box control to notify its parent about events. The application determines the child-window identifier; it must be unique for all child windows with the same parent window.</param>
    /// <param name="hInstance">A handle to the instance of the module to be associated with the window.</param>
    /// <param name="lpParam">
    /// <para>Pointer to a value to be passed to the window through the <see cref="CREATESTRUCT"/> structure (lpCreateParams member) pointed to by the lParam param of the WM_CREATE message. This message is sent to the created window by this function before it returns.</para>
    /// <para>If an application calls CreateWindow to create a MDI client window, lpParam should point to a <see cref="CLIENTCREATESTRUCT"/> structure. If an MDI client window calls CreateWindow to create an MDI child window, lpParam should point to a <see cref="MDICREATESTRUCT"/> structure. lpParam may be NULL if no additional data is needed.</para>
    /// </param>
    /// <returns>
    /// <para>If the function succeeds, the return value is a handle to the new window.</para>
    /// <para>If the function fails, the return value is NULL. To get extended error information, call GetLastError.</para>
    /// <para>This function typically fails for one of the following reasons:</para>
    /// <list type="bullet">
    /// <item>an invalid parameter value</item>
    /// <item>the system class was registered by a different module</item>
    /// <item>The <see cref="WH_CBT"/> hook is installed and returns a failure code</item>
    /// <item>if one of the controls in the dialog template is not registered, or its window window procedure fails <see cref="WINDOW_MESSAGE.WM_CREATE"/> or <see cref="WINDOW_MESSAGE.WM_NCCREATE"/></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>The <see cref="CreateWindowEx"/> function sends <see cref="WINDOW_MESSAGE.WM_NCCREATE"/>, <see cref="WINDOW_MESSAGE.WM_NCCALCSIZE"/>, and <see cref="WINDOW_MESSAGE.WM_CREATE"/> messages to the window being created.</para>
    /// <para>If the created window is a child window, its default position is at the bottom of the Z-order. If the created window is a top-level window, its default position is at the top of the Z-order (but beneath all topmost windows unless the created window is itself topmost).</para>
    /// </remarks>
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
    /// Calls the default window procedure to provide default processing for any window messages that an application does not process.
    /// This function ensures that every message is processed. DefWindowProc is called with the same parameters received by the window procedure.
    /// </summary>
    /// <param name="hWnd">A handle to the window procedure that received the message.</param>
    /// <param name="uMsg">The message.</param>
    /// <param name="wParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
    /// <param name="lParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
    /// <returns>The return value is the result of the message processing and depends on the message.</returns>
    [LibraryImport(nameof(User32))]
    public static partial LRESULT DefWindowProcW(HWND hWnd, WINDOW_MESSAGE uMsg, WPARAM wParam, LPARAM lParam);

    /// <summary>
    /// <para>
    /// Destroys the specified window. The function sends <see cref="WINDOW_MESSAGE.WM_DESTROY"/> and <see cref="WINDOW_MESSAGE.WM_NCDESTROY"/> messages to the window to deactivate it and remove the keyboard focus from it.
    /// The function also destroys the window's menu, flushes the thread message queue, destroys timers, removes clipboard ownership, and breaks the clipboard viewer chain (if the window is at the top of the viewer chain).
    /// </para>
    /// <para>
    /// If the specified window is a parent or owner window, DestroyWindow automatically destroys the associated child or owned windows when it destroys the parent or owner window. The function first destroys child or owned windows,
    /// and then it destroys the parent or owner window.
    /// </para>
    /// <para>DestroyWindow also destroys modeless dialog boxes created by the CreateDialog function.</para>
    /// </summary>
    /// <param name="hWnd">A handle to the window to be destroyed.</param>
    /// <returns>
    /// <para>If the function succeeds, the return value is nonzero.</para>
    /// <para>If the function fails, the return value is zero. To get extended error information, call <see cref="Kernel32.GetLastError"/>.</para>
    /// </returns>
    /// <remarks>
    /// <para>A thread cannot use DestroyWindow to destroy a window created by a different thread.</para>
    /// <para>If the window being destroyed is a child window that does not have the <see cref="WINDOW_STYLES_EX.WS_EX_NOPARENTNOTIFY"/> style, a <see cref="WINDOW_MESSAGE.WM_PARENTNOTIFY"/> message is sent to the parent.</para>
    /// </remarks>
    /// <example>For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/using-windows">Destroying a Window</see>.</example>
    [LibraryImport(nameof(User32))]
    public static partial BOOL DestroyWindow(HWND hWnd);

    /// <summary>
    /// Dispatches a message to a window procedure. It is typically used to dispatch a message retrieved by the GetMessage function.
    /// </summary>
    /// <param name="lpMsg">A pointer to a structure that contains the message.</param>
    /// <returns>The return value specifies the value returned by the window procedure. Although its meaning depends on the message being dispatched, the return value generally is ignored.</returns>
    /// <remarks>
    /// <para>The MSG structure must contain valid message values. If the lpmsg parameter points to a WM_TIMER message and the lParam parameter of the WM_TIMER message is not NULL, lParam points to a function that is called instead of the window procedure.</para>
    /// <para>Note that the application is responsible for retrieving and dispatching input messages to the dialog box. Most applications use the main message loop for this. However, to permit the user to move to and to select controls by using the keyboard, the application must call IsDialogMessage. For more information, see Dialog Box Keyboard Interface.</para>
    /// </remarks>
    /// <example>
    /// For an example, see <see href="https://learn.microsoft.com/en-us/windows/desktop/winmsg/using-messages-and-message-queues">Creating a Message Loop</see>.
    /// </example>
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

    [LibraryImport(nameof(User32))]
    public static partial int GetSystemMetrics(SYSTEM_METRIC smIndex);

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

    [LibraryImport(nameof(User32))]
    public static partial BOOL TranslateMessage(in MSG lpMsg);

    [LibraryImport(nameof(User32))]
    public static partial HWND WindowFromPoint(POINT point);
}
