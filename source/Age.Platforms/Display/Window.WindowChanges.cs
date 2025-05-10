namespace Age.Platforms.Display;

public partial class Window
{
    [Flags]
    private enum WindowChanges
    {
        None     = 0,
        Close    = 1 << 0,
        Size     = 1 << 1,
        Position = 1 << 2,
    }
}
