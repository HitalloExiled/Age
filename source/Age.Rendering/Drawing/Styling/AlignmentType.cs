namespace Age.Rendering.Drawing.Styling;

[Flags]
public enum AlignmentType
{
    BaseLine = 0,
    Center   = 1 << 0,
    Left     = 1 << 1,
    Right    = 1 << 2,
    Top      = 1 << 3,
    Bottom   = 1 << 4,
}
