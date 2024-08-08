using Age.Numerics;

using KeyEvent        = System.Action<Age.Platforms.Display.Key>;
using MouseClickEvent = System.Action<Age.Platforms.Display.MouseButton>;
using MouseMoveEvent  = System.Action<short, short>;
using MouseWhellEvent = System.Action<float, Age.Platforms.Display.MouseKeyStates>;

namespace Age.Platforms.Display;

public partial class Window : IDisposable
{
    public event MouseClickEvent? ClickDown;
    public event MouseClickEvent? ClickUp;
    public event MouseClickEvent? DoubleClick;
    public event KeyEvent?        KeyDown;
    public event KeyEvent?        KeyPress;
    public event KeyEvent?        KeyUp;
    public event MouseClickEvent? Click;
    public event MouseMoveEvent?  MouseMove;
    public event MouseWhellEvent? MouseWhell;
    public event Action?          SizeChanged;
    public event Action?          WindowClosed;

    private static string? className;

    protected static readonly Dictionary<nint, Window> WindowsMap = [];

    protected readonly List<Window> Children = [];

    public static IEnumerable<Window> Windows => WindowsMap.Values;

    private Point<int> position;
    private Size<uint> size;
    private string     title;
    private bool       disposed;

    protected static bool Registered { get; set; }

    public Size<uint> ClientSize => this.PlatformGetClientSize();
    public Window?    Parent     { get; }

    public bool Closed    { get; private set; }
    public nint Handle    { get; private set; }
    public bool Maximized { get; private set; }
    public bool Minimized { get; private set; }
    public bool Visible   { get; private set; } = true;

    public Point<int> Position { get => this.position; set => this.PlatformSetPosition(value); }
    public Size<uint> Size     { get => this.size;     set => this.PlatformSetSize(value); }
    public string     Title    { get => this.title;    set => this.PlatformSetTitle(value); }

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

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.Close();
            }

            this.disposed = true;
        }
    }

    public void Close()
    {
        if (!this.Closed)
        {
            this.PlatformClose();

            this.Parent?.Children.Remove(this);
            WindowClosed?.Invoke();

            this.Closed = true;
        }
    }

    public void DoEvents() =>
        this.PlatformDoEvents();

    public void Hide()
    {
        this.PlatformHide();

        this.Visible = false;
    }

    public void Maximize()
    {
        this.PlatformMaximize();

        this.Maximized = true;
        this.Minimized = false;
    }

    public void Minimize()
    {
        this.PlatformMinimize();

        this.Maximized = false;
        this.Minimized = true;
    }

    public void Restore()
    {
        this.PlatformRestore();

        this.Maximized = false;
        this.Minimized = false;
    }

    public void Show()
    {
        this.PlatformShow();

        this.Visible = true;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
