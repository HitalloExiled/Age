using Age.Core.Collections;
using Age.Core;
using Age.Numerics;
using System.Diagnostics.CodeAnalysis;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager : Disposable
{
    public string Id { get; }

    public partial Cursor Cursor { get; set; }

    public partial int  CursorScale   { get; set; }
    public partial bool CursorVisible { get; set; }

    [AllowNull]
    public static WindowManager Instance { get; private set; }

    public partial WindowManager(string id);

    protected override partial void OnDisposed(bool disposing);

    internal partial void CloseWindow(Window window);
    internal partial WindowState* CreateWindow(string title, Size<int> size, Window? parent);
    internal partial NativeArray<WindowMessage> FlushWindowEvents(Window window);
    internal partial string? GetClipboardData(Window window);
    internal partial void HideWindow(Window window);
    internal partial void MaximizeWindow(Window window);
    internal partial void MinimizeWindow(Window window);
    internal partial void RestoreWindow(Window window);
    internal partial void SetWindowClipboardData(Window window, string value);
    internal partial void SetWindowTitle(Window window, string value);
    internal partial void ShowWindow(Window window);
    internal partial void UpdateCursor();
}
