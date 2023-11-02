using Age.Platform.Windows.Native.Types;

namespace Age.Platform.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// Contains window class information. It is used with the <see cref="RegisterClassExW(in WNDCLASSEXW)"/> and <see cref="GetClassInfoEx"/> functions.
    /// The <see cref="WNDCLASSEXW"/> structure is similar to the <see cref="WNDCLASS"/> structure. There are two differences. <see cref="WNDCLASSEXW"/> includes the <see cref="cbSize"/> member, which specifies the size of the structure, and the <see cref="hIconSm"/> member, which contains a handle to a small icon associated with the window class.
    /// </summary>
    public unsafe struct WNDCLASSEXW
    {
        /// <summary>
        /// The size, in bytes, of this structure. Set this member to <c>sizeof(WNDCLASSEX)</c>. Be sure to set this member before calling the <see cref="GetClassInfoEx"/> function.
        /// </summary>
        public UINT cbSize;

        /// <summary>
        /// The class style(s). This member can be any combination of the Class Styles.
        /// </summary>
        public CLASS_STYLES style;

        /// <summary>
        /// A pointer to the window procedure. You must use the <see cref="CallWindowProc"/> function to call the window procedure. For more information, see <see cref="WindowProc"/>.
        /// </summary>
        public WNDPROC lpfnWndProc;

        /// <summary>
        /// The number of extra bytes to allocate following the window-class structure. The system initializes the bytes to zero.
        /// </summary>
        public int cbClsExtra;

        /// <summary>
        /// The number of extra bytes to allocate following the window instance. The system initializes the bytes to zero. If an application uses <see cref="WNDCLASSEX"/> to register a dialog box created by using the CLASS directive in the resource file, it must set this member to <see cref="DLGWINDOWEXTRA"/>.
        /// </summary>
        public int cbWndExtra;

        /// <summary>
        /// A handle to the instance that contains the window procedure for the class.
        /// </summary>
        public HINSTANCE hInstance;

        /// <summary>
        /// A handle to the class icon. This member must be a handle to an icon resource. If this member is NULL, the system provides a default icon.
        /// </summary>
        public HICON hIcon;

        /// <summary>
        /// A handle to the class cursor. This member must be a handle to a cursor resource. If this member is NULL, an application must explicitly set the cursor shape whenever the mouse moves into the application's window.
        /// </summary>
        public HCURSOR hCursor;

        /// <summary>
        /// <para>
        /// A handle to the class background brush.
        /// This member can be a handle to the brush to be used for painting the background, or it can be a color value. A color value must be one of the following standard system colors (the value 1 must be added to the chosen color). If a color value is given, you must convert it to one of the following HBRUSH types:
        /// <list type="bullet">
        /// <item>COLOR_ACTIVEBORDER</item>
        /// <item>COLOR_ACTIVECAPTION</item>
        /// <item>COLOR_APPWORKSPACE</item>
        /// <item>COLOR_BACKGROUND</item>
        /// <item>COLOR_BTNFACE</item>
        /// <item>COLOR_BTNSHADOW</item>
        /// <item>COLOR_BTNTEXT</item>
        /// <item>COLOR_CAPTIONTEXT</item>
        /// <item>COLOR_GRAYTEXT</item>
        /// <item>COLOR_HIGHLIGHT</item>
        /// <item>COLOR_HIGHLIGHTTEXT</item>
        /// <item>COLOR_INACTIVEBORDER</item>
        /// <item>COLOR_INACTIVECAPTION</item>
        /// <item>COLOR_MENU</item>
        /// <item>COLOR_MENUTEXT</item>
        /// <item>COLOR_SCROLLBAR</item>
        /// <item>COLOR_WINDOW</item>
        /// <item>COLOR_WINDOWFRAME</item>
        /// <item>COLOR_WINDOWTEXT</item>
        /// </list>
        /// </para>
        /// <para>The system automatically deletes class background brushes when the class is unregistered by using <see cref="UnregisterClass"/>. An application should not delete these brushes.</para>
        /// <para>
        /// When this member is NULL, an application must paint its own background whenever it is requested to paint in its client area.
        /// To determine whether the background must be painted, an application can either process the <see cref="WINDOW_MESSAGE.WM_ERASEBKGND"/> message or test the fErase member of the <see cref="PAINTSTRUCT"/> structure filled by the <see cref="BeginPaint"/> function.
        /// </para>
        /// </summary>
        public HBRUSH hbrBackground;

        /// <summary>
        /// Pointer to a null-terminated character string that specifies the resource name of the class menu, as the name appears in the resource file. If this member is NULL, windows belonging to this class have no default menu.
        /// </summary>
        public LPCWSTR lpszMenuName;

        /// <summary>
        /// A pointer to a null-terminated string or is an atom. If this parameter is an atom, it must be a class atom created by a previous call to the <see cref="RegisterClass"/> or <see cref="RegisterClassEx"/> function. The atom must be in the low-order word of lpszClassName; the high-order word must be zero.
        /// </summary>
        public LPCWSTR lpszClassName;

        /// <summary>
        /// A handle to a small icon that is associated with the window class. If this member is NULL, the system searches the icon resource specified by the hIcon member for an icon of the appropriate size to use as the small icon.
        /// </summary>
        public HICON hIconSm;
    }
}
