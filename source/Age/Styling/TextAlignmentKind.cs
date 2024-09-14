namespace Age.Styling;

[Flags]
public enum TextAlignmentKind
{
    None     = 0,
    Center   = 1 << 1,
    Left     = 1 << 3,
    Right    = 1 << 4,
    Top      = 1 << 5,
    Bottom   = 1 << 6,
}
