namespace Age.Styling;

internal partial class StyledStateManager
{
    public event Action<StyleProperty>? Changed;

    private Style?        style;
    private bool          isDirty;
    private State         states;
    private StyledStates? styles;

    private State States
    {
        get => this.states;
        set
        {
            if (this.states != value)
            {
                static bool hasValue(Style? style, State state, State previous, State current) =>
                    style != null && (previous.HasFlag(state) || current.HasFlag(state));

                var shouldNotify =
                    hasValue(this.Styles?.Focus,    State.Focus,    this.states, value)
                 || hasValue(this.Styles?.Hovered,  State.Hovered,  this.states, value)
                 || hasValue(this.Styles?.Disabled, State.Disabled, this.states, value)
                 || hasValue(this.Styles?.Enabled,  State.Enabled,  this.states, value)
                 || hasValue(this.Styles?.Checked,  State.Checked,  this.states, value)
                 || hasValue(this.Styles?.Active,   State.Active,   this.states, value);

                this.states = value;

                if (shouldNotify)
                {
                    this.InvokeChanged();
                }
            }
        }
    }

    public Style Style
    {
        get
        {
            this.style ??= new();

            if (this.isDirty)
            {
                if (this.Styles?.Base == null)
                {
                    this.style.Clear();
                }
                else
                {
                    this.style.Copy(this.Styles.Base);
                }

                if (this.States.HasFlag(State.Focus) && this.Styles?.Focus != null)
                {
                    this.style.Merge(this.Styles.Focus);
                }

                if (this.States.HasFlag(State.Hovered) && this.Styles?.Hovered != null)
                {
                    this.style.Merge(this.Styles.Hovered);
                }

                if (this.States.HasFlag(State.Disabled) && this.Styles?.Disabled != null)
                {
                    this.style.Merge(this.Styles.Disabled);
                }

                if (this.States.HasFlag(State.Enabled) && this.Styles?.Enabled != null)
                {
                    this.style.Merge(this.Styles.Enabled);
                }

                if (this.States.HasFlag(State.Checked) && this.Styles?.Checked != null)
                {
                    this.style.Merge(this.Styles.Checked);
                }

                if (this.States.HasFlag(State.Active) && this.Styles?.Active != null)
                {
                    this.style.Merge(this.Styles.Active);
                }

                this.isDirty = false;
            }

            return this.style;
        }
    }

    public StyledStates? Styles
    {
        get => this.styles;
        set
        {
            if (this.styles != value)
            {
                if (this.styles != null)
                {
                    this.styles.Changed -= this.InvokeChanged;
                }

                if (value != null)
                {
                    value.Base ??= this.styles?.Base;
                    value.Changed += this.InvokeChanged;
                }

                this.styles = value;

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
