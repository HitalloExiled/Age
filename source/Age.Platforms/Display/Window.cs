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

    private string        title;
    private WindowChanges windowChanges;

    protected List<Window> Children { get; } = [];

    public bool IsClosed    { get; private set; }
    public bool IsMaximized { get; private set; }
    public bool IsMinimized { get; private set; }
    public bool IsVisible   { get; private set; } = true;

    public partial Cursor Cursor   { get; set; }
    public partial string Title    { get; set; }

    public Window? Parent { get; }

    public partial Size<uint> Size     { get; }
    public partial nint       Surface  { get; }

    public partial Window(string? title = default, Size<uint>? size = default, Window? parent = null);

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Close();
        }
    }

    internal partial void UpdateCursor();

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
