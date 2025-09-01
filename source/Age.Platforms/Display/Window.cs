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
    public event Action?              Resized;
    #endregion events

    private static string? className;

    protected static Dictionary<nint, Window> WindowsMap { get; } = [];

    public static IEnumerable<Window> Windows => WindowsMap.Values;

    protected static bool Registered { get; set; }

    private Point<int>    position;
    private Size<uint>    size;
    private string        title;
    private WindowChanges windowChanges;

    protected List<Window> Children { get; } = [];

    public Window? Parent { get; }

    public nint Handle      { get; private set; }
    public bool IsClosed    { get; private set; }
    public bool IsMaximized { get; private set; }
    public bool IsMinimized { get; private set; }
    public bool IsVisible   { get; private set; } = true;

    public Size<uint> ClientSize => this.PlatformGetClientSize();

    public Cursor Cursor
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                PlatformSetCursor(value);
            }
        }
    }

    public Point<int> Position
    {
        get => this.position;
        set
        {
            if (this.position != value)
            {
                this.position = value;
                this.PlatformSetPosition(value);
            }
        }
    }

    public Size<uint> Size
    {
        get => this.size;
        set
        {
            if (this.size != value)
            {
                this.size = value;
                this.PlatformSetSize(value);
            }
        }
    }

    public string Title
    {
        get => this.title;
        set
        {
            if (this.title != value)
            {
                this.title = value;
                this.PlatformSetTitle(value);
            }
        }
    }

    public Window(string title, Size<uint> size, Point<int> position, Window? parent = null)
    {
        this.title    = title;
        this.size     = size;
        this.position = position;
        this.Parent   = parent;

        this.PlatformCreate(title, size, position, parent);

        parent?.Children.Add(this);
    }

    public static void Register(string className)
    {
        if (Registered)
        {
            throw new Exception("Windows class already registered");
        }

        PlatformRegister(className);

        Registered = true;

        Window.className = className;
    }

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

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Close();
        }
    }

    public void Close()
    {
        if (!this.IsClosed)
        {
            this.PlatformClose();

            this.Parent?.Children.Remove(this);
            Closed?.Invoke();

            this.IsClosed = true;
        }
    }

    public string? GetClipboardData() =>
        this.PlatformGetClipboardData();

    public void SetClipboardData(string value) =>
        this.PlatformSetClipboardData(value);

    public void DoEvents() =>
        this.PlatformDoEvents();

    public void Hide()
    {
        this.PlatformHide();

        this.IsVisible = false;
    }

    public void Maximize()
    {
        this.PlatformMaximize();

        this.IsMaximized = true;
        this.IsMinimized = false;
    }

    public void Minimize()
    {
        this.PlatformMinimize();

        this.IsMaximized = false;
        this.IsMinimized = true;
    }

    public void Restore()
    {
        this.PlatformRestore();

        this.IsMaximized = false;
        this.IsMinimized = false;
    }

    public void Show()
    {
        this.PlatformShow();

        this.IsVisible = true;
    }
}
