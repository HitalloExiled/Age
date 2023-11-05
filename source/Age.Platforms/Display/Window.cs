using Age.Numerics;

namespace Age.Platforms.Display;

public abstract class Window(string title, Size<uint> size, Point<int> position, Window? parent = null)
{
    public event Action? WindowClosed;
    public event Action? SizeChanged;

    public bool Closed    { get; protected set; }
    public bool Maximized { get; protected set; }
    public bool Minimized { get; protected set; }
    public bool Visible   { get; protected set; }

    public Window? Parent { get; protected set; } = parent;

    public Point<int>  Position   { get; set; } = position;
    public Size<uint>  Size       { get; set; } = size;
    public string      Title      { get; set; } = title;

    public abstract Size<uint> ClientSize { get; }

    public abstract void Close();

    public abstract void Hide();

    public abstract void Maximize();

    public abstract void Minimize();

    public abstract void Restore();

    public abstract void Show();

    protected void NotifyClosed() =>
        this.WindowClosed?.Invoke();

    protected void NotifySizeChanged() =>
        this.SizeChanged?.Invoke();
}
