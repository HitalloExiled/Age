namespace Age.Styling;

internal partial class StyledStateManager
{
    [Flags]
    public enum State
    {
        None     = 0,
        Active   = 1 << 0,
        Checked  = 1 << 1,
        Disabled = 1 << 2,
        Enabled  = 1 << 3,
        Focus    = 1 << 4,
        Hovered  = 1 << 5,
    }
}
