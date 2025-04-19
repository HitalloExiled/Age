namespace Age.Commands;

public static partial class CommandPool
{
    public static MeshCommandPool MeshCommand { get; } = new();
    public static RectCommandPool RectCommand { get; } = new();
    public static TextCommandPool TextCommand { get; } = new();
}
