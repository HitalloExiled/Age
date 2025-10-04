namespace Age.Commands;

[Flags]
public enum CommandFilter : uint
{
    None      = 0,
    Color     = 1 << 0,
    Index     = 1 << 1,
    Wireframe = 1 << 2,
}
