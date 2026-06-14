namespace Age.Platforms.Display;

[Flags]
public enum Modifier : byte
{
    None = 0,

    Alt   = 1 << 0,
    Shift = 1 << 1,
    Ctrl  = 1 << 2,
    Meta  = 1 << 3,
}
