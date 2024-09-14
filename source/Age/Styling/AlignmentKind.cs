namespace Age.Styling;

[Flags]
public enum AlignmentKind
{
    None     = 0,
    Begin    = 1 << 0,
    Center   = 1 << 1,
    End      = 1 << 2,
    Left     = 1 << 3,
    Right    = 1 << 4,
    Top      = 1 << 5,
    Bottom   = 1 << 6,
}
