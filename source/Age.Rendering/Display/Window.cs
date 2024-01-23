using System.Diagnostics.CodeAnalysis;
using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Vulkan;

namespace Age.Rendering.Display;

public partial class Window : IDisposable
{
    private static readonly Dictionary<nint, Window> windows = [];

    private static string?         className;
    private static VulkanRenderer? renderer;

    private readonly List<Window> children = [];

    [MemberNotNullWhen(true, nameof(renderer))]
    private static bool Registered { get; set; }

    private Point<int> position;
    private Size<uint> size;
    private string     title;
    private bool       disposed;

    public event Action? SizeChanged;
    public event Action? WindowClosed;

    public static IEnumerable<Window> Windows => windows.Values;

    public Size<uint> ClientSize => this.PlatformGetClientSize();
    public Content    Content    { get; } = new();
    public Window?    Parent     { get; }

    public bool           Closed    { get; private set; }
    public SurfaceContext Context   { get; private set; }
    public bool           Maximized { get; private set; }
    public bool           Minimized { get; private set; }
    public bool           Visible   { get; private set; } = true;

    public Point<int>    Position { get => this.position; set => this.PlatformSetPosition(value); }
    public Size<uint>    Size     { get => this.size;     set => this.PlatformSetSize(value); }
    public string        Title    { get => this.title;    set => this.PlatformSetTitle(value); }

    public Window(string title, Size<uint> size, Point<int> position, Window? parent = null)
    {
        this.title    = title;
        this.size     = size;
        this.position = position;
        this.Parent   = parent;

        if (!Registered)
        {
            throw new Exception("Windows class not registered");
        }

        this.PlatformCreate(title, size, position, parent);

        parent?.children.Add(this);
    }

    public static void Register(string className, VulkanRenderer context)
    {
        if (Registered)
        {
            throw new Exception("Windows class already registered");
        }

        PlatformRegister(className);

        Registered = true;
        renderer   = context;

        Window.className = className;
    }

    public static void CloseAll()
    {
        foreach (var window in windows.Values)
        {
            window.Close();
        }

        windows.Clear();
    }

    public static void DoEventsAll()
    {
        foreach (var window in windows.Values)
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

            this.Parent?.children.Remove(this);
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
