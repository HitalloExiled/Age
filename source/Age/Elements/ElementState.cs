namespace Age.Elements;

[Flags]
public enum ElementState
{
    None     = 0,
    Active   = 1 << 0,
    Checked  = 1 << 1,
    Disabled = 1 << 2,
    Enabled  = 1 << 3,
    Focused  = 1 << 4,
    Hovered  = 1 << 5,
}
