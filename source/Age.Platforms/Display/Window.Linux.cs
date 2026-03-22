#if LINUX
using Age.Numerics;

namespace Age.Platforms.Display;

public partial class Window
{
    public partial Size<uint> ClientSize => throw new NotImplementedException();

    public partial Cursor Cursor
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            UpdateCursor();
        }
    }

    public partial Point<int> Position
    {
        get => this.position;
        set => throw new NotImplementedException();
    }

    public partial Size<uint> Size
    {
        get => this.size;
        set => throw new NotImplementedException();
    }

    public partial string Title
    {
        get => this.title;
        set => throw new NotImplementedException();
    }

    private partial void UpdateCursor() => throw new NotImplementedException();

    public static unsafe partial void Register(string className) => throw new NotImplementedException();

    public partial void Close() => throw new NotImplementedException();
    private unsafe partial void Create(string title, Size<uint> size, Point<int> position, Window? parent) => throw new NotImplementedException();
    public partial void DoEvents() => throw new NotImplementedException();
    public partial string? GetClipboardData() => throw new NotImplementedException();
    public partial void Hide() => throw new NotImplementedException();
    public partial void Maximize() => throw new NotImplementedException();
    public partial void Minimize() => throw new NotImplementedException();
    public partial void Restore() => throw new NotImplementedException();
    public partial void SetClipboardData(string value) => throw new NotImplementedException();
    public partial void Show() => throw new NotImplementedException();
}
#endif
