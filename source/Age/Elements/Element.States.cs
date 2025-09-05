using Age.Core.Extensions;

namespace Age.Elements;

public abstract partial class Element
{
    [Flags]
    internal enum State : byte
    {
        None     = 0,
        Active   = 1 << 0,
        Checked  = 1 << 1,
        Disabled = 1 << 2,
        Enabled  = 1 << 3,
        Focused  = 1 << 4,
        Hovered  = 1 << 5,
    }

    private State ActiveStates
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (this.StyleSheet != null)
                {
                    this.ComputeStyle(this.ComputedStyle.Data);
                }
            }
        }
    }

    public bool IsActive  => this.ActiveStates.HasFlags(State.Active);
    public bool IsFocused => this.ActiveStates.HasFlags(State.Focused);
    public bool IsHovered => this.ActiveStates.HasFlags(State.Hovered);

    private protected void AddState(State state) =>
        this.ActiveStates |= state;

    private protected void RemoveState(State state) =>
        this.ActiveStates &= ~state;
}
