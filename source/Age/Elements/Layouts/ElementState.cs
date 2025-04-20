namespace Age.Elements.Layouts;

[Flags]
public enum ElementState
{
    None     = 0,
    Active   = 1 << 0,
    Checked  = 1 << 1,
    Disabled = 1 << 2,
    Enabled  = 1 << 3,
    Focus    = 1 << 4,
    Hovered  = 1 << 5,
}
