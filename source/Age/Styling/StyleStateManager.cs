namespace Age.Styling;

internal partial class StyleStateManager
{
    public event Action<StyleProperty>? Changed;

    private Style?        current;
    private bool          isDirty;
    private State         states;
    private StyledStates? styledStates;

    private State States
    {
        get => this.states;
        set
        {
            if (this.states != value)
            {
                this.states = value;
                this.InvokeChanged();
            }
        }
    }

    private Style? @base;

    internal Style Current
    {
        get
        {
            this.current ??= new();

            if (this.isDirty)
            {
                if (this.Base != null)
                {
                    this.current.Data = this.Base.Data;
                }

                if (this.States.HasFlag(State.Focus) && this.StyledStates?.Focus != null)
                {
                    this.current.Data.Append(this.StyledStates.Focus.Data);
                }

                if (this.States.HasFlag(State.Hovered) && this.StyledStates?.Hovered != null)
                {
                    this.current.Data.Append(this.StyledStates.Hovered.Data);
                }

                if (this.States.HasFlag(State.Disabled) && this.StyledStates?.Disabled != null)
                {
                    this.current.Data.Append(this.StyledStates.Disabled.Data);
                }

                if (this.States.HasFlag(State.Enabled) && this.StyledStates?.Enabled != null)
                {
                    this.current.Data.Append(this.StyledStates.Enabled.Data);
                }

                if (this.States.HasFlag(State.Checked) && this.StyledStates?.Checked != null)
                {
                    this.current.Data.Append(this.StyledStates.Checked.Data);
                }

                if (this.States.HasFlag(State.Active) && this.StyledStates?.Active != null)
                {
                    this.current.Data.Append(this.StyledStates.Active.Data);
                }
            }

            return this.current;
        }
    }

    internal Style? Base
    {
        get => this.@base;
        set
        {
            if (this.@base != value)
            {
                if (this.@base != null)
                {
                    this.@base.Changed -= this.InvokeChanged;
                }

                this.@base = value;

                if (this.@base != null)
                {
                    this.@base.Changed += this.InvokeChanged;
                }

                this.InvokeChanged();
            }
        }
    }

    public StyledStates? StyledStates
    {
        get => this.styledStates;
        set
        {
            if (this.styledStates != value)
            {
                if (this.styledStates != null)
                {
                    this.styledStates.Changed -= this.InvokeChanged;
                }

                if (value != null)
                {
                    value.Changed += this.InvokeChanged;
                }

                this.styledStates = value;

                this.InvokeChanged();
            }
        }
    }

    private void InvokeChanged(StyleProperty property = StyleProperty.All)
    {
        this.isDirty = true;
        this.Changed?.Invoke(property);
    }

    public void AddState(State state) =>
        this.States |= state;

    public void RemoveState(State state) =>
        this.States &= ~state;
}
