namespace Age.Styling;

public class StyledStates
{
    private Style? @base;
    private Style? @checked;
    private Style? active;
    private Style? disabled;
    private Style? enabled;
    private Style? focus;
    private Style? hovered;

    public event Action<StyleProperty>? Changed;

    public Style? Active
    {
        get => this.active;
        set => this.Set(ref this.active, value);
    }

    public Style? Base
    {
        get => this.@base;
        set => this.Set(ref this.@base, value);
    }

    public Style? Checked
    {
        get => this.@checked;
        set => this.Set(ref this.@checked, value);
    }

    public Style? Disabled
    {
        get => this.disabled;
        set => this.Set(ref this.disabled, value);
    }

    public Style? Enabled
    {
        get => this.enabled;
        set => this.Set(ref this.enabled, value);
    }

    public Style? Focus
    {
        get => this.focus;
        set => this.Set(ref this.focus, value);
    }

    public Style? Hovered
    {
        get => this.hovered;
        set => this.Set(ref this.hovered, value);
    }

    private void InvokeChanged(StyleProperty property) =>
        Changed?.Invoke(property);

    private void Set(ref Style? field, Style? value)
    {
        if (field != value)
        {
            if (field != null)
            {
                field.Changed -= this.InvokeChanged;
            }

            if (value != null)
            {
                value.Changed += this.InvokeChanged;
            }

            field = value;

            this.InvokeChanged(StyleProperty.All);
        }
    }
}
