namespace Age.Styling;

using Age.Core.Extensions;

internal partial class StyledStateManager
{
    public event Action<StyleProperty>? Changed;

    private Style?        style;
    private bool          isDirty;
    private State         states;
    private StyledStates? styles;
    private Style?        userStyle;

    private State States
    {
        get => this.states;
        set
        {
            if (this.states != value)
            {
                static bool hasValue(Style? style, State state, State previous, State current) =>
                    style != null && (previous.HasFlags(state) || current.HasFlags(state));

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
                    this.InvokeChanged(); // TODO: implements style properties differ
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
                    this.style.Clear(false);
                }
                else
                {
                    this.style.Copy(this.Styles.Base, false);
                }

                if (this.UserStyle != null)
                {
                    this.style.Merge(this.UserStyle, false);
                }

                if (this.States.HasFlags(State.Focus) && this.Styles?.Focus != null)
                {
                    this.style.Merge(this.Styles.Focus, false);
                }

                if (this.States.HasFlags(State.Hovered) && this.Styles?.Hovered != null)
                {
                    this.style.Merge(this.Styles.Hovered, false);
                }

                if (this.States.HasFlags(State.Disabled) && this.Styles?.Disabled != null)
                {
                    this.style.Merge(this.Styles.Disabled, false);
                }

                if (this.States.HasFlags(State.Enabled) && this.Styles?.Enabled != null)
                {
                    this.style.Merge(this.Styles.Enabled, false);
                }

                if (this.States.HasFlags(State.Checked) && this.Styles?.Checked != null)
                {
                    this.style.Merge(this.Styles.Checked, false);
                }

                if (this.States.HasFlags(State.Active) && this.Styles?.Active != null)
                {
                    this.style.Merge(this.Styles.Active, false);
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
                this.styles = value;

                this.InvokeChanged();
            }
        }
    }

    public Style? UserStyle
    {
        get => this.userStyle;
        set
        {
            if (this.userStyle != value)
            {
                if (this.userStyle != null)
                {
                    this.userStyle.Changed -= this.InvokeChanged;
                }

                if (value != null)
                {
                    value.Changed += this.InvokeChanged;
                }

                this.userStyle = value;

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
