namespace Age.Commands;

[Flags]
public enum CommandFilter : uint
{
    None      = 0,
    Color     = 1 << 0,
    Encode    = 1 << 1,
    Wireframe = 1 << 2,
}
