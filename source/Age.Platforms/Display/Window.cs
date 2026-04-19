using Age.Core;
using Age.Numerics;

namespace Age.Platforms.Display;

public delegate void WindowMouseEventHandler(in WindowMouseEvent mouseEvent);
public delegate void WindowContextEventHandler(in WindowContextEvent mouseEvent);
public delegate void WindowKeyEventHandler(Key key);
public delegate void WindowInputEventHandler(char character);

public partial class Window : Disposable
{
    #region events
    public event WindowMouseEventHandler?   Click;
    public event Action?                    Closed;
    public event WindowContextEventHandler? Context;
    public event WindowMouseEventHandler?   DoubleClick;
    public event WindowInputEventHandler?   Input;
    public event WindowKeyEventHandler?     KeyDown;
    public event WindowKeyEventHandler?     KeyPress;
    public event WindowKeyEventHandler?     KeyUp;
    public event WindowMouseEventHandler?   MouseDown;
    public event WindowMouseEventHandler?   MouseMove;
    public event WindowMouseEventHandler?   MouseUp;
    public event WindowMouseEventHandler?   MouseWheel;
    public event Action?                    Resized;
    #endregion events

    private static Size<uint> defaultSize = new(800, 600);
    private static string?    appId;

    private string        title;
    private WindowChanges windowChanges;

    public static IEnumerable<Window> Windows => WindowsMap.Values;

    protected static bool                     Registered { get; set; }
    protected static Dictionary<nint, Window> WindowsMap { get; } = [];

    protected List<Window> Children { get; } = [];

    public Window? Parent { get; }

    public nint Handle      { get; private set; }
    public bool IsClosed    { get; private set; }
    public bool IsMaximized { get; private set; }
    public bool IsMinimized { get; private set; }
    public bool IsVisible   { get; private set; } = true;

    public partial Size<uint> ClientSize { get; }
    public partial Cursor     Cursor     { get; set; }
    public partial Point<int> Position   { get; set; }
    public partial Size<uint> Size       { get; set; }
    public partial string     Title      { get; set; }

    public partial Window(string? title = default, Size<uint>? size = default, Point<int>? position = default, Window? parent = null);

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Close();
        }
    }

    private partial void UpdateCursor();

    public static void CloseAll()
    {
        foreach (var window in WindowsMap.Values)
        {
            window.Close();
        }

        WindowsMap.Clear();
    }

    public static void DoEventsAll()
    {
        foreach (var window in WindowsMap.Values)
        {
            window.DoEvents();
        }
    }

    public static partial void Register(string appId);
    public static partial void Destroy();

    public partial void Close();
    public partial string? GetClipboardData();
    public partial void DoEvents();
    public partial void Hide();
    public partial void Maximize();
    public partial void Minimize();
    public partial void Restore();
    public partial void SetClipboardData(string value);
    public partial void Show();
}
