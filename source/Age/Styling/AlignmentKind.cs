namespace Age.Styling;

[Flags]
public enum AlignmentKind
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

public enum FontWeight
{
    None       =	0,
    Thin       = 100,
    ExtraLight = 200,
    Light      = 300,
    Normal     = 400,
    Medium     = 500,
    SemiBold   = 600,
    Bold       = 700,
    ExtraBold  = 800,
    Black      = 900,
    ExtraBlack = 1000,
}
