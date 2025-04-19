namespace Age.Styling;

[Flags]
public enum Alignment : byte
{
    None     = 0,
    Baseline = 1 << 0,
    Start    = 1 << 1,
    Center   = 1 << 2,
    End      = 1 << 3,
    Left     = 1 << 4,
    Right    = 1 << 5,
    Top      = 1 << 6,
    Bottom   = 1 << 7,
}
