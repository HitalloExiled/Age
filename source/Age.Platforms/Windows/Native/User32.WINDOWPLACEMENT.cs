
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static unsafe partial class User32
{
    /// <summary>
    /// <para>Contains information about the placement of a window on the screen.</para>
    /// <remarks>
    /// <para>If the window is a top-level window that does not have the <see cref="WINDOW_STYLES_EX.WS_EX_TOOLWINDOW"/> window style, then the coordinates represented by the following members are in workspace coordinates: ptMinPosition, ptMaxPosition, and rcNormalPosition. Otherwise, these members are in screen coordinates.</para>
    /// <para>Workspace coordinates differ from screen coordinates in that they take the locations and sizes of application toolbars (including the taskbar) into account. Workspace coordinate (0,0) is the upper-left corner of the workspace area, the area of the screen not being used by application toolbars.</para>
    /// <para>The coordinates used in a <see cref="WINDOWPLACEMENT"/> structure should be used only by the <see cref="GetWindowPlacement"/> and <see cref="SetWindowPlacement"/> functions. Passing workspace coordinates to functions which expect screen coordinates (such as <see cref="SetWindowPos"/>) will result in the window appearing in the wrong location. For example, if the taskbar is at the top of the screen, saving window coordinates using <see cref="GetWindowPlacement"/> and restoring them using <see cref="SetWindowPos"/> causes the window to appear to "creep" up the screen.</para>
    /// </remarks>
    /// </summary>
    public struct WINDOWPLACEMENT
    {
        /// <summary>
        /// The length of the structure, in bytes. Before calling the <see cref="GetWindowPlacement"/> or <see cref="SetWindowPlacement"/> functions, set this member to sizeof(WINDOWPLACEMENT).
        /// </summary>
        public UINT length;

        /// <summary>
        /// <para>The length of the structure, in bytes. Before calling the <see cref="GetWindowPlacement"/> or <see cref="SetWindowPlacement"/> functions, set this member to sizeof(WINDOWPLACEMENT).</para>
        /// <para><see cref="GetWindowPlacement"/> and <see cref="SetWindowPlacement"/> fail if this member is not set correctly.</para>
        /// </summary>
        public UINT flags;

        /// <summary>
        /// The current show state of the window. It can be any of the values that can be specified in the nCmdShow parameter for the <see cref="ShowWindow"/> function.
        /// </summary>
        public SHOW_WINDOW_COMMANDS showCmd;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is minimized.
        /// </summary>
        public POINT ptMinPosition;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is maximized.
        /// </summary>
        public POINT ptMaxPosition;

        /// <summary>
        /// The window's coordinates when the window is in the restored position.
        /// </summary>
        public RECT rcNormalPosition;

        public RECT rcDevice;
    }
}
