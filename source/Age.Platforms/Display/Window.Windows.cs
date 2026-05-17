#if WINDOWS
using Age.Numerics;

namespace Age.Platforms.Display;

public unsafe partial class Window
{
    public nint Handle => this.State->Handle;

    public partial Size<uint> Size => this.State->Size.Cast<uint>();

    public partial Cursor Cursor
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            WindowManager.Instance.UpdateCursor(field = value);
        }
    }

    public partial string Title
    {
        get => this.title;
        set
        {
            if (this.title != value)
            {
                WindowManager.Instance.SetWindowTitle(this, value);

                this.title = value;
            }
        }
    }

    public partial Window(string? title, Size<uint>? size, Window? parent)
    {
        this.title  = title ?? "Untitled";
        this.Parent = parent;

        parent?.children.Add(this);

        this.State = WindowManager.Instance.CreateWindow(this.title, (size ?? defaultSize).Cast<int>(), parent);
    }
}
#endif
